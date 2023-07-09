param environment string
param resourceGroupName string
param location string
param principalId string
param storageAccountSku string
param redisCacheSku string
param redisCacheFamily string
param redisCacheCapacity int
param appServicePlanSku string
param appServicePlanTier string
param appServicePlanCapacity int
param appConfigurationSku string
param metricThreshold int
param appServiceMetricName string
param storageAccountMetricName string
param redisCacheMetricName string
param cosmosDbMetricName string
param keyVaultMetricName string
param appServiceTimeAggregation string
param storageAccountTimeAggregation string
param redisCacheTimeAggregation string
param cosmosDbTimeAggregation string
param keyVaultTimeAggregation string
param evaluationFrequency string
param windowSize string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${environment}-lab-appservice-managed-identity'
  location: location
}

// Create a storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: '${environment}labserverlessstorage'
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageAccountSku
  }
  properties: {
    isHnsEnabled: true
  }
  identity: {
    type: 'SystemAssigned, UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
}

// Create a Redis Cache
resource redisCache 'Microsoft.Cache/Redis@2022-06-01' = {
  name: '${environment}-lab-appservice-redis'
  location: location
  properties: {
    sku: {
      name: redisCacheSku
      family: redisCacheFamily
      capacity: redisCacheCapacity
    }
  }
  identity: {
    type: 'SystemAssigned, UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
}

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
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
  identity: {
    type: 'SystemAssigned,UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
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
        objectId: principalId
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
    name: appServicePlanSku
    tier: appServicePlanTier
    capacity: appServicePlanCapacity
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
  identity: {
    type: 'SystemAssigned, UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: '${environment}-lab-app-config'
  location: location
  sku: {
    name: appConfigurationSku
  }
  identity: {
    type: 'SystemAssigned, UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
}

// Assign IAM role
/* resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroupName, 'Contributor')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c') // Contributor role
    principalId: principalId
    principalType: 'User'
    scope: resourceGroup().id
  }
} */

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
    evaluationFrequency: evaluationFrequency
    windowSize: windowSize
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: appServiceMetricName
          operator: 'GreaterThan'
          threshold: metricThreshold
          timeAggregation: appServiceTimeAggregation
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
    evaluationFrequency: evaluationFrequency
    windowSize: windowSize
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: storageAccountMetricName
          operator: 'GreaterThan'
          threshold: metricThreshold
          timeAggregation: storageAccountTimeAggregation
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
    evaluationFrequency: evaluationFrequency
    windowSize: windowSize
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: redisCacheMetricName
          operator: 'GreaterThan'
          threshold: metricThreshold
          timeAggregation: redisCacheTimeAggregation
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
    evaluationFrequency: evaluationFrequency
    windowSize: windowSize
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: cosmosDbMetricName
          operator: 'GreaterThan'
          threshold: metricThreshold
          timeAggregation: cosmosDbTimeAggregation
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
    evaluationFrequency: evaluationFrequency
    windowSize: windowSize
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          criterionType:'StaticThresholdCriterion'
          name: 'Metric1'
          metricName: keyVaultMetricName
          operator: 'GreaterThan'
          threshold: metricThreshold
          timeAggregation: keyVaultTimeAggregation
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

// Assign IAM role
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroupName, 'Contributor')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c') // Contributor role
    principalId: principalId
    principalType: 'User'
    scope: resourceGroup().id
  }
  dependsOn: [
    managedIdentity
    storageAccount
    redisCache
    cosmosDB
    keyVault
    appServicePlan
    appService
    appConfiguration
    appInsights
    actionGroup
    appServiceAlertRule
    StorageAlertRule
    RedisAlertRule
    CosmosAlertRule
    keyVaultAlertRule
  ]
}
