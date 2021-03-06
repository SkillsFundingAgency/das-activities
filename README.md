# Activities

`SFA.DAS.Activities.Worker` subscribes to events via Azure Service Bus and indexes them via Elastic Search. `SFA.DAS.Activities.Client` allows the data to be queried via Elastic Search.

|               |               |
| ------------- | ------------- |
| ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) | SFA.DAS.Activities |
| Build | ![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge) |

|               |               |
| ------------- | ------------- |
| ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) | SFA.DAS.Activities.Client |
| Client | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Activities.Client)](https://www.nuget.org/packages/SFA.DAS.Activities.Client) |

## Requirements

1. Install [Visual Studio].
2. Install [Choclatey]:

```PowerShell
> iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))
```

3. Install [Docker]:

```PowerShell
> choco install docker-for-windows
```

4. Install [Elastic Search] image:

```PowerShell
> docker pull docker.elastic.co/elasticsearch/elasticsearch:5.6.5
```

5. Increase Docker Memory Allocation to 3500 in System Notification Area:  

```
Docker (Right Click) -> Settings -> Advanced -> Memory - slide to 3584 or more
```

## Run

1. Run [Elastic Search] container:

```PowerShell
> cd .\tools\elasticsearch.5.6.5
> docker-compose up -d
```

2. Open the solution.
3. Set `SFA.DAS.Activities.Worker` as the startup project.
4. Browse to `~\AppData\Roaming`.
5. Add a directory named `SFA.DAS.Activities`.
6. Add a directory to the above directory named `add_paye_scheme`.
7. Add a file to the above directory named `PayeSchemeAddedMessage.json` containing the following json:

```JavaScript
{
    "accountId": 5,
    "createdAt": "2017-01-01T12:00:00.000Z",
    "creatorUserRef": "04FCDEC7-5758-4BD2-A2D4-3E288E9EE047",
    "creatorName": "John Doe",
    "payeScheme": "333/AA00001"
}
```
8. Set `SFA.DAS.Activities.MessageHandlers` as startup project
9. Hit F5 to run the job.
10. HTTP GET http://localhost:9200/local-activities/_search

[Choclatey]: https://chocolatey.org
[Docker]: https://www.docker.com
[Elastic Search]: https://www.elastic.co/products/elasticsearch
[Visual Studio]: https://www.visualstudio.com
