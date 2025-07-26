param(
    [Parameter(Mandatory=$true)]
    [string]$OrganizationUrl,
    
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,
    
    [Parameter(Mandatory=$true)]
    [string]$PatToken,

    [Parameter(Mandatory=$true)]
    [string]$AwsAccessKeyId,

    [Parameter(Mandatory=$true)]
    [string]$AwsSecretAccessKey,

    [Parameter(Mandatory=$true)]
    [string]$AwsAccountId
)

# Base64 encode PAT token for authentication
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($PatToken)"))

# Create header with authorization
$headers = @{
    Authorization = "Basic $base64AuthInfo"
    'Content-Type' = 'application/json'
}

# Variable group definition
$variableGroup = @{
    name = "aws-credentials"
    type = "Vsts"
    variables = @{
        AWS_ACCESS_KEY_ID = @{
            value = $AwsAccessKeyId
            isSecret = $true
        }
        AWS_SECRET_ACCESS_KEY = @{
            value = $AwsSecretAccessKey
            isSecret = $true
        }
        AWS_ACCOUNT_ID = @{
            value = $AwsAccountId
            isSecret = $false
        }
    }
}

# Convert to JSON
$jsonBody = $variableGroup | ConvertTo-Json -Depth 10

# API URL for creating variable group
$apiUrl = "$OrganizationUrl/$ProjectName/_apis/distributedtask/variablegroups?api-version=7.1-preview.2"

try {
    # Create variable group
    $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Headers $headers -Body $jsonBody
    Write-Host "Variable group 'aws-credentials' created successfully with ID: $($response.id)"
}
catch {
    Write-Error "Failed to create variable group: $_"
    exit 1
}
