### Overview


* App uses OpenTelemetry packages OpenTelemetry packages to gather traces. Packages: OpenTelemetry, OpenTelemetry.Exporter.Jaeger, 
OpenTelemetry.Instrumentation.AspNetCore, OpenTelemetry.Instrumentation.SqlClient
* .NET core for application. version 3.1
* MS SQL for application database
* Jaeger to gather traces from app.
* Elasticsearch to store traces
* Kibana for nice UI to gather info on traces.
* Docker to containerize ms sql, jaeger, kibana and elasticsearch

![Overview](https://github.com/arvikangas/tracing/blob/main/images/Overview.png)

### Description

.NET core app uses MS SQL database. OpenTelemetry packages gather data from sql internal libraries and send it to Jaeger. Because jaeger has no nice UI to aggregate traces or make nice graphs, i used kibana and elasticsearch to store traces. Code-wise i did not do much. Only some API controller code, some database code and i changed tracing so it adds sql query parameters to trace too. Most time consuming was jaeger, elasticsearch and kibana. Elasticsearch ignores fields that are over 200 chars when aggregating data. I had to read some docs to change default configuration.

Finding slowest sql queries could be done also from Jaeger, but without UI. Jaeger has API endpoints to query data but then I'd have to build somekinda UI to show them. I also needed elasticsearch to store traces. So i went lazy route and added Kibana that has better UI.

### End result

![Result](https://github.com/arvikangas/tracing/blob/main/images/Result.png)

### To Run

* start services with docker-compose up
* start application after services are up. In directory ./SqlApp/SqlApp run command dotnet run
* run some http queries against application. For example GET http://localhost:5000/orders. Http queries are saved in /queries folder
* Watch traces appear in: Jaeger http://localhost:16686 and Kibana http://localhost:5601
* NB! i have not added Kibana configuration. Kibana requires choosing index from elasticsearch and also configuration update (for elasticsearch) so it would aggregate on sql query text.
