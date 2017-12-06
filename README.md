# Digital Apprenticeships Service

## Activities Worker

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Activities Worker|
| Build | ![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge) |

### Developer Setup

- run Elastic search 5.6.4 on port 9200 

### Setup

#### Config

	ConfigurationStorageConnectionString

Required to connect to table storage

    Environment:Name
    ElasticSearch:BaseUrl
    ElasticSearch:UserName
    ElasticSearch:Password
    ServiceBus:ConnectionString

- Checks table storage
- If you add them to the cscfg replace : with _
