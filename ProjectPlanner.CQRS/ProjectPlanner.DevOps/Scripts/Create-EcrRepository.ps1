param(
    [Parameter(Mandatory=$true)]
    [string]$AwsRegion,

    [Parameter(Mandatory=$true)]
    [string]$RepositoryName
)

try {
    # Check if AWS CLI is installed
    if (-not (Get-Command aws -ErrorAction SilentlyContinue)) {
        throw "AWS CLI is not installed. Please install it first."
    }

    # Check if AWS credentials are configured
    $awsCredentials = aws configure list
    if (-not $?) {
        throw "AWS credentials are not configured. Please run 'aws configure' first."
    }

    Write-Host "Creating ECR repository: $RepositoryName"

    # Create ECR repository
    $createRepo = aws ecr create-repository --repository-name $RepositoryName --region $AwsRegion
    if (-not $?) {
        throw "Failed to create ECR repository"
    }

    # Enable image scanning
    $scanningConfig = aws ecr put-image-scanning-configuration `
        --repository-name $RepositoryName `
        --image-scanning-configuration scanOnPush=true `
        --region $AwsRegion
    if (-not $?) {
        Write-Warning "Failed to enable image scanning, but repository was created"
    }

    # Add lifecycle policy to clean up untagged images
    $lifecyclePolicy = @{
        rules = @(
            @{
                rulePriority = 1
                description = "Remove untagged images older than 14 days"
                selection = @{
                    tagStatus = "untagged"
                    countType = "sinceImagePushed"
                    countUnit = "days"
                    countNumber = 14
                }
                action = @{
                    type = "expire"
                }
            }
        )
    } | ConvertTo-Json -Depth 10

    $lifecyclePolicyResult = aws ecr put-lifecycle-policy `
        --repository-name $RepositoryName `
        --lifecycle-policy-text $lifecyclePolicy `
        --region $AwsRegion
    if (-not $?) {
        Write-Warning "Failed to set lifecycle policy, but repository was created"
    }

    Write-Host "ECR repository created successfully!"
    Write-Host "Repository URI: $($createRepo | ConvertFrom-Json | Select-Object -ExpandProperty repository | Select-Object -ExpandProperty repositoryUri)"
}
catch {
    Write-Error $_.Exception.Message
    exit 1
}
