param resourceGroupName string = 'tst-lab-appservice-serverless'
param location string = 'uksouth'


resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'tst-lab-appservice-managed-identity'
  location: location
}

// Create a storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: 'tstlabserverlessstorage'
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
  name: 'tst-lab-appservice-redis'
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
  name: 'tst-lab-appservice-cosmosdb'
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
  name: 'tst-lab-appservice-kv'
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
          keys: [
            'create'
            'get'
          ]
        }
      }
    ]
  }
}


// Create an App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'tst-lab-appservice-plan'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    capacity: 1
  }
}

// Create an App Service
resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: 'tst-lab-appservice'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      windowsFxVersion: 'DOTNETCORE|6.0'
    }
  }
}

// Create a Virtual Network
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2022-07-01' = {
  name: 'tst-lab-vnet'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'tst-lab-subnet'
        properties: {
          addressPrefix: '10.0.1.0/24'
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Disabled'
        }
      }
    ]
  }
}

resource privateEndpointStorage 'Microsoft.Network/privateEndpoints@2022-07-01' = {
  name: 'storageAccountPrivateEndpoint'
  location: location
  properties: {
    subnet: {
      id: '${virtualNetwork.id}/subnets/tst-lab-subnet'
    }
    privateLinkServiceConnections: [
      {
        name: 'storageAccountPrivateLinkConnection'
        properties: {
          privateLinkServiceId: storageAccount.id
          groupIds: [
            'blob'
          ]
        }
      }
    ]
  }
}

resource redisPrivateEndpoint 'Microsoft.Network/privateEndpoints@2020-07-01' = {
  name: 'tst-lab-appservice-redis-pe'
  location: location
  properties: {
    subnet: {
      id: '${virtualNetwork.id}/subnets/tst-lab-subnet'
    }
    privateLinkServiceConnections: [
      {
        name: 'redisPrivateLinkConnection'
        properties: {
          privateLinkServiceId: redisCache.id
          groupIds: [
            'redisCache'
          ]
        }
      }
    ]
  }
}

resource cosmosDBPrivateEndpoint 'Microsoft.Network/privateEndpoints@2020-07-01' = {
  name: 'tst-lab-appservice-cosmosdb-pe'
  location: location
  properties: {
    subnet: {
      id: '${virtualNetwork.id}/subnets/tst-lab-subnet'
    }
    privateLinkServiceConnections: [
      {
        name: 'cosmosdbPrivateLinkConnection'
        properties: {
          privateLinkServiceId: cosmosDB.id
          groupIds: [
            'MongoDB'
          ]
        }
      }
    ]
  }
}


resource keyVaultPrivateEndpoint 'Microsoft.Network/privateEndpoints@2022-07-01' = {
  name: 'tst-lab-appservice-kv-pe'
  location: location
  properties: {
    subnet: {
      id: '${virtualNetwork.id}/subnets/tst-lab-subnet'
    }
    privateLinkServiceConnections: [
      {
        name: 'keyVaultPrivateLinkConnection'
        properties: {
          privateLinkServiceId: keyVault.id
          groupIds: [
            'vault'
          ]
        }
      }
    ]
  }
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  name: 'tst-lab-app-config'
  location: location
  sku: {
    name: 'standard'
  }
  properties: {
   /*  encryption: {
      keyVaultProperties: {
        keyIdentifier: 'https://${keyVault.name}.${environment().suffixes.keyvaultDns}/keys/cosmoskey'
 
      }
    } */
  }
}

resource appConfigurationPrivateEndpoint 'Microsoft.Network/privateEndpoints@2022-07-01' = {
  name: 'appConfigurationPrivateEndpoint'
  location: location
  properties: {
    subnet: {
      id: '${virtualNetwork.id}/subnets/tst-lab-subnet'
    }
    privateLinkServiceConnections: [
      {
        name: 'appConfigurationPrivateLinkConnection'
        properties: {
          privateLinkServiceId: appConfiguration.id
          groupIds: [
            'configurationStores'
          ]
        }
      }
    ]
  }
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2018-09-01' = {
  name: 'privatelink.azurewebsites.net'
  location: 'global'
}

resource privateDnsZoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2022-07-01' = {
  name: 'tst-appConfigurationPrivateDnsZoneGroup'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'appConfigurationPrivateDnsZoneConfig'
        properties: {
          privateDnsZoneId: privateDnsZone.id
        }
      }
    ]
  }
  parent: appConfigurationPrivateEndpoint
}

// Define private DNS zone link
resource privateDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2018-09-01' = {
  parent: privateDnsZone
  name: '${virtualNetwork.name}Link'
  location: 'global' 
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
}
