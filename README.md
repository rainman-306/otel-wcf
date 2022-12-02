# otel-wcf
POC using WCF instrumentation with OpenTelemetry collector

Original code was forked from https://github.com/CoreWCF/samples 

Consists of 2 projects:
| Project | Description |
|---------|-------------|
| NetFrameworkServer | .NET Framework WCF Server  |
| NetFrameworkClient | .NET Framework WCF Client |

Original code taken from https://github.com/CoreWCF/samples, was modified to include NetNamedPipeBinding, the OpenTelemetry WCF Instrumentation, and OpenTelemetry Collector.

## Run ZipKin in Docker Container

`docker run -d -p 9411:9411 openzipkin/zipkin`

## Setting up OpenTelemetry Collector

Download the otecol from https://github.com/open-telemetry/opentelemetry-collector-releases/releases, extract contents into a directory.  

Copy the `otelcol.yaml` configuration file into the same directory the contents were extracted into.

Launch the OpenTelemetry Collector in PowerShell console

`.\otelcol.exe --config .\otelcol.yaml`
