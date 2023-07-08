param environment string = 'dev'
param resourceGroupName string = '${environment}-lab-appservice-serverless'
param location string = 'uksouth'

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: '${environment}-lab-appservice-managed-identity'
  location: location
}

// Create a storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: '${environment}labserverlessstorage'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    isHnsEnabled: true
  }
}

// Create a Redis Cache
resource redisCache 'Microsoft.Cache/Redis@2022-06-01' = {
  name: '${environment}-lab-appservice-redis'
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
  }
}

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: '${environment}-lab-appservice-cosmosdb'
  location: location
  kind: 'MongoDB'
  properties: {
    databaseAccountOfferType: 'Standard' 
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
  }
}

// Create a Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: '${environment}-lab-appservice-kv'
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: 'f4af1079-a94f-4e43-814a-3d63e1f44f94' 
        permissions: {
          keys: ['Get', 'List', 'Update', 'Create', 'Import', 'Delete', 'Recover', 'Backup', 'Restore']
     
        }
      }
    ]
  }
}

// Create an App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${environment}-lab-appservice-plan'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    capacity: 1
  }
}

// Create an App Service
resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: '${environment}-lab-appservice'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      windowsFxVersion: 'DOTNETCORE|6.0'
    }
  }
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  name: '${environment}-lab-app-config'
  location: location
  sku: {
    name: 'standard'
  }
}

// Assign IAM role
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroupName, 'Contributor')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c') // Contributor role
    principalId: 'f4af1079-a94f-4e43-814a-3d63e1f44f94'
    principalType: 'User'
    scope: resourceGroup().id
  }
}

// Create an Application Insights instance
resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: '${environment}-lab-appservice-appinsights'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Create an Azure Monitor Action Group
resource actionGroup 'Microsoft.Insights/actionGroups@2018-03-01' = {
  name: '${environment}-lab-appservice-actiongroup'
  location: 'Global'
  properties: {
    groupShortName: 'actiongroup'
    enabled: true
  }
}

// Create an Azure Monitor Alert Rule appService
resource appServiceAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${environment}labappservicealertrule'
  location: 'Global'
  properties: {
    description: 'Alert rule'
    severity: 3
    enabled: true
    scopes: [
      appService.id
    ]
    evaluationFrequency: 'PT1H'
    windowSize: 'PT1H'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: 'CpuTime'
          operator: 'GreaterThan'
          threshold: 90
          timeAggregation: 'Count'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}

// Create an Azure Monitor Alert Rule storageAccount
resource StorageAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${environment}labstoragealertruleappservice'
  location: 'Global'
  properties: {
    description: 'Alert rule'
    severity: 3
    enabled: true
    scopes: [
      storageAccount.id
    ]
    evaluationFrequency: 'PT1H'
    windowSize: 'PT1H'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: 'UsedCapacity'
          operator: 'GreaterThan'
          threshold: 90
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}

// Create an Azure Monitor Alert Rule redis
resource RedisAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${environment}labredisalertruleappservice'
  location: 'Global'
  properties: {
    description: 'Alert rule'
    severity: 3
    enabled: true
    scopes: [
      redisCache.id
    ]
    evaluationFrequency: 'PT1H'
    windowSize: 'PT1H'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: 'CacheHits'
          operator: 'GreaterThan'
          threshold: 90
          timeAggregation: 'Count'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}

// Create an Azure Monitor Alert Rule cosmos
resource CosmosAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${environment}labcosmosalertruleappservice'
  location: 'Global'
  properties: {
    description: 'Alert rule'
    severity: 3
    enabled: true
    scopes: [
      cosmosDB.id
    ]
    evaluationFrequency: 'PT1H'
    windowSize: 'PT1H'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: 'TotalRequestUnits'
          operator: 'GreaterThan'
          threshold: 90
          timeAggregation: 'Total'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}
// Create an Azure Monitor Alert Rule keyVault
resource keyVaultAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${environment}labkeyvaultalertruleappservice'
  location: 'Global'
  properties: {
    description: 'Alert rule'
    severity: 3
    enabled: true
    scopes: [
      keyVault.id
    ]
    evaluationFrequency: 'PT1H'
    windowSize: 'PT1H'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: 'ServiceApiLatency'
          operator: 'GreaterThan'
          threshold: 90
          timeAggregation: 'Count'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}
// Create an Azure Monitor Alert Rule appConfig
/* resource appConfigAlertRule 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${environment}labappconfigalertruleappservice'
  location: 'Global'
  properties: {
    description: 'Alert rule'
    severity: 3
    enabled: true
    scopes: [
      appConfiguration.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: 'CpuTime'
          operator: 'GreaterThan'
          threshold: 90
          timeAggregation: 'Average'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
} */
