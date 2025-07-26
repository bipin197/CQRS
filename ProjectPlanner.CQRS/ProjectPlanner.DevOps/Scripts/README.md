# Example usage of the scripts

# 1. Create ECR Repository
.\Create-EcrRepository.ps1 `
    -AwsRegion "us-east-1" `
    -RepositoryName "projectplanner-api"

# 2. Create Azure DevOps Variable Group
.\Create-VariableGroup.ps1 `
    -OrganizationUrl "https://dev.azure.com/YourOrganization" `
    -ProjectName "YourProject" `
    -PatToken "YourPersonalAccessToken" `
    -AwsAccessKeyId "YourAwsAccessKeyId" `
    -AwsSecretAccessKey "YourAwsSecretAccessKey" `
    -AwsAccountId "YourAwsAccountId"
