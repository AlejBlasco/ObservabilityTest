La observabilidad en el contexto de un sistema distribuido es la capacidad de supervisar y analizar la telemetría sobre el estado de cada componente, para poder observar los cambios en el rendimiento y diagnosticar por qué se producen esos cambios. 

A diferencia de la depuración, que es invasiva y puede afectar al funcionamiento de la aplicación, la observabilidad pretende ser transparente para el funcionamiento principal y tener un impacto en el rendimiento lo suficientemente pequeño como para que pueda utilizarse de forma continua.

# ¿Cómo se enfoca en .NET?

Alguna de las maneras diferentes de lograr la observabilidad en las aplicaciones de .NET son:

- Explícitamente en el código, consultando y utilizando una biblioteca como OpenTelemetry. Si tiene acceso al código fuente y puede recompilar la aplicación, este es el mecanismo más eficaz y configurable.
- Fuera de proceso mediante EventPipe. Las herramientas como dotnet-monitor pueden escuchar registros y métricas y, a continuación, procesarlos sin afectar al código.
- Usando un gancho de inicio, se pueden inyectar montajes en el proceso que luego pueden recopilar instrumentación. Un ejemplo de este enfoque es la Instrumentación automática de OpenTelemetry .NET.

# ¿Qué es OpenTelemetry?

**OpenTelemetry** (_OTel_) es un estándar multiplataforma abierto para recopilar y emitir datos de telemetría. OpenTelemetry incluye:

- [API](https://opentelemetry.io/docs/concepts/instrumentation/manual/) para bibliotecas que se pueden utilizar para registrar datos de telemetría mientras se ejecuta el código.
- APIs que los desarrolladores de aplicaciones utilizan para configurar qué parte de los datos registrados se enviarán a través de la red, a dónde se enviarán y cómo pueden filtrarse, almacenarse en búfer, enriquecerse y transformarse.
- Las [convenciones semánticas](https://github.com/open-telemetry/semantic-conventions) proporcionan orientación sobre la denominación y el contenido de los datos telemétricos. Es importante que las aplicaciones que producen datos telemétricos y las herramientas que reciben los datos se pongan de acuerdo sobre lo que significan los distintos tipos de datos y qué clase de datos son útiles para que las herramientas puedan proporcionar un análisis eficaz.
- Una interfaz para exportadores. Los exportadores son complementos que permiten transmitir datos de telemetría en formatos específicos a diferentes back-end de telemetría.
- El protocolo de conexión OTLP es una opción de protocolo de red neutral del proveedor para transmitir datos de telemetría. Algunas herramientas y proveedores admiten este protocolo además de los protocolos propietarios preexistentes que pueden tener.
El uso de OTel permite el uso de una amplia variedad de sistemas APM, incluidos sistemas de código abierto como Prometheus y Grafana, Azure Monitor: producto de APM de Microsoft en Azure o de muchos proveedores de APM que se asocian con OpenTelemetry.

# Los 3 pilares
Los tres pilares de la observabilidad son:
## Logs
Registros que indican operaciones individuales, como una solicitud, un error, o un mensaje.

Para poder trabajar con logs en .Net, necesitamos usar la interfaz ILogger. Esta interfaz nos permite escribir mensajes de logs en nuestra aplicación. Y para tener esta interfaz, necesitaremos añadir el paquete Microsoft.Extensions.Logging:

`dotnet add package Microsoft.Extensions.Logging`

Un ILogger por sí mismo no funciona. Necesitamos un proveedor de logs que se encargue de escribir los mensajes en algún sitio. Por ejemplo, podemos usar el proveedor que escribe los mensajes en la consola añadiendo este paquete:

`dotnet add package Microsoft.Extensions.Logging.Console`

Y para poder instanciar un ILogger, necesitamos un ILoggerFactory. Este ILoggerFactory es el que se encarga de crear los ILogger que necesitemos


```
using var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(_ => true)
                                                                 .AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

logger.LogDebug("Hello from debug");
await Task.Delay(1);
logger.LogInformation("Hello from info");
await Task.Delay(1);
logger.LogWarning("Hello from warn");
await Task.Delay(1);
logger.LogError("Hello from error");
await Task.Delay(1);
logger.LogCritical("Hello from critical");
await Task.Delay(1);
```


## Métricas

Las métricas son simplemente medidas numéricas que nos permiten conocer el estado de algo en particular. En el caso de una aplicación, esto podría ser el número de solicitudes por segundo, la cantidad de memoria utilizada, el tiempo de respuesta, entre otras cosas.

Las métricas son importantes porque nos permiten detectar problemas en nuestra aplicación y solucionarlos antes de que se conviertan en un problema mayor. Además, también nos ayudan a entender mejor cómo se comporta nuestra aplicación, los usuarios dentro de la misma y cómo podemos mejorarla.

Para esto, vamos a necesitar el paquete de nuget System.Diagnostics.DiagnosticSource, que es la librería recomendada para crear métricas personalizadas en .Net en las últimas versiones del runtime.

Estas librerías sustituyen a las antiguas EventCounters y System.Diagnostics.PerformanceCounter.


```
using System.Diagnostics.Metrics;

var myMeter = new Meter("beer-meter");
var beersDrank = myMeter.CreateCounter<int>("beers-meter-drank");
```
Existen varios tipos de métricas:
- **Counter**. Métrica básica, cuenta el número de veces que ocurre un elemento.
- **UpDownCounter**. Permite aumentar y disminuir el valor.
- **ObservableCounter**. Permite registrar una función para recoger el valor.
- **ObservableUpDownCounter**. Mezcla de las 2 anteriores.
- **ObservableGauge**. Valor que se evalúa en un momento determinado.
- **Histogram**. Permite medir la distribución.


##Trazas
Las trazas distribuidas son una técnica de diagnóstico que ayuda a los desarrolladores y operadores a localizar fallos y problemas de rendimiento en aplicaciones, especialmente aquellas que pueden estar distribuidas en múltiples máquinas o procesos. Esta técnica sigue las solicitudes a través de una aplicación, correlacionando el trabajo realizado por diferentes componentes que la forman y separándolo del trabajo que puede estar haciendo para solicitudes concurrentes.

Para utilizarla, debemos instalar el paquete NuGet System.Diagnostics.DiagnosticSource en nuestro proyecto:

`dotnet add package System.Diagnostics.DiagnosticSource`

Una vez instalado, tendremos que referenciar el espacio de nombres System.Diagnostics en nuestro código:

`using System.Diagnostics;`

Esto nos dará acceso a la clase ActivitySource, que es la que nos permitirá generar trazas en nuestra aplicación:

var source = new ActivitySource("beer-tracing");
La clase ActivitySource nos permite crear una fuente de trazas con un nombre determinado. A partir de una instancia de esta clase, podemos crear nuevas trazas utilizando el método StartActivity:

```
using (var activity = source.StartActivity("drink-beer"))
{
    // Do something
}
```

# Conectividad
EL primer paso será asegurarnos de que el proyecto en el que queramos añadir OpenTelemetry tiene los paquetes necesarios para esto mismo (_estos son los paquetes necesarios a día de 25/01/2024_).


```
  <ItemGroup>
    <PackageReference Include="Azure.Monitor.OpenTelemetry.AspNetCore" Version="1.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.7.0" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.7.0" />
    <PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.5.0-rc.1" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.7.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.7.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.7.0" />
  </ItemGroup>
```

Para poder acceder a los datos generados, tendremos que configurar OpenTelemetry en .NET, esto lo podemos hacer mediante inyección de dependencias, en el siguiente código se aprecia todos los elementos necesarios y en cada uno de ellos un comentario de que hace (_es un ejemplo activando todo lo posible para obtener mas información y exportar a N sitios_)


```
public static IServiceCollection AddTelemetry(this IServiceCollection services,
    WebApplicationBuilder builder)
{
    var otel = builder.Services.AddOpenTelemetry();

    // Configure OpenTelemetry Resources with the application name
    var apiMetricsMeterName = builder.Configuration["Metrics:ApiMetrics:Meter:Name"];

    otel.ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName));

    // Add Azure Monitor
    otel.UseAzureMonitor();

    // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
    otel.WithMetrics(metrics => metrics
        // Metrics provider from OpenTelemetry
        .AddAspNetCoreInstrumentation()
        // Custom metrics
        .AddMeter(apiMetricsMeterName!)
        // Metrics provides by ASP.NET Core in .NET 8
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        // Set Prometheus
        .AddPrometheusExporter());

    // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
    var apiMetricsActivitySourceName = builder.Configuration["Metrics:ApiMetrics:ActivitySource:Name"];
    var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];

    otel.WithTracing(tracing =>
    {
        // Default traces
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        // Custom traces
        tracing.AddSource(apiMetricsActivitySourceName!);
        // Set Endpoints
        if (tracingOtlpEndpoint != null)
        {
            tracing.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
            });
        }
        // Add Console Endpoint
        tracing.AddConsoleExporter();
    });

    return services;
}
```
## Acceso a las métricas
Si se ha configurado correctamente nuestro proyecto, se habrá habilitado el endpoint **/metrics** para consultar esta información (algunos de los siguientes programas usan ese endpoint para capturar los datos).

![image.png](/.attachments/image-9b0ba8b3-c724-445b-ba0e-bb7bc0ba67d4.png)

## Consola del proyecto
Al igual que en el punto anterior, al haber configurado correctamente OpenTelemetry y teniendo la siguiente linea en la inyección de dependencias se podrán visualizar las trazas por consola.


```
// Add Console Endpoint
tracing.AddConsoleExporter();
```

**NOTA:** Esta línea no es necesaria para un correcto funcionamiento, lo mejor es comentarla o activarla solo en caso de estar en desarrollo o cuando no se tengan más endpoints disponibles

## Prometheus
El primer paso será descargar e instalar Prometheus es en equipo o servidor siguiendo este [enlace](https://prometheus.io/download/)

Luego añadiremos la URL del sitio que queremos "observar" en el *.yaml de configuración y ejecutaremos el programa


```
scrape_configs:
  # The job name is added as a label `job=<job_name>` to any timeseries scraped from this config.
  - job_name: "prometheus"
    - scrape_interval: 1s # poll very quickly for a more responsive demo

    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.

    scrape_interval: 1s # poll very quickly for a more responsive demo
    static_configs:
      - targets: ["localhost:5212"]
```

![image.png](/.attachments/image-a05b3e0b-938f-41d6-ad4d-3a30c57dd10a.png)

Con esto, podremos ir al puerto :9090 y tendremos acceso a Prometheus

![image.png](/.attachments/image-a4b7825c-80ba-49bf-b1a2-78b89b8c924b.png)

## Jager
De manera similar a Prometheus, descargaremos Jaeger desde el siguiente [enlace](https://www.jaegertracing.io/download/)

Con el programa instalado, ejecutaremos el jaeger-all-in-one.exe, y en la salida de consola veremos el puerto al que tendremos que acceder para visualizar la información (_por defecto es :4317_)

![image.png](/.attachments/image-0ae3c803-a406-4081-ab82-55294295e59e.png)

## App Insight / Azure Monitor
Quizás, la conectividad más compleja de este tutorial; lo primero es asegurarnos de que tenemos la inyección de dependencias configurada con los siguientes elementos:


```
var otel = builder.Services.AddOpenTelemetry();
otel.UseAzureMonitor();
otel.WithMetrics(metrics => metrics
    .AddMeter(greeterMeter.Name)
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel"));
otel.WithTracing(tracing =>
{
    tracing.AddSource(greeterActivitySource.Name);
});
```
Después, si aún no es un cliente de Azure, puede crear una cuenta gratuita en https://azure.microsoft.com/free/. Inicie sesión en Azure Portal y seleccione un recurso de Application Insights existente o cree uno con https://ms.portal.azure.com/#create/Microsoft.AppInsights.

Application Insights identifica la instancia que se va a usar para almacenar y procesar datos a través de una clave de instrumentación y una cadena de conexión que se encuentran en la parte superior derecha de la interfaz de usuario del portal.

<IMG  src="https://learn.microsoft.com/es-es/dotnet/core/diagnostics/media/portal_ui.thumb.png"  alt="Connection String in Azure Portal"/>

Si usa Azure App Service, esta cadena de conexión se pasa automáticamente a la aplicación como una variable de entorno. Para otros servicios o cuando se ejecute localmente, deberá pasarlo utilizando la variable de entorno APPLICATIONINSIGHTS_CONNECTION_STRING o en appsettings.json. Para ejecutar localmente, es más fácil agregar el valor a appsettings.json:

`"APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=12345678-abcd-abcd-abcd-12345678..."`

Al ejecutar la aplicación, la telemetría se enviará a Application Insights. 

![image.png](/.attachments/image-d3cb3270-2d81-4451-b866-249e2d38d799.png)


# Ejemplo practico
En este [enlace](https://github.com/AlejBlasco/ObservabilityTest), se encuentra un proyecto API .NET de pruebas que se ha realizado a medida que se han ido recopilando los datos para tener una primera visión de la observabilidad.

Se trata de una simulación muy light de un carrito de compra con usuarios y productos, en el tenemos métricas de los carritos y los productos, trazas de las operaciones así como logs de las llamadas a los métodos (además de todos los indicadores proporcionados nativamente por OpenTelemetry).

# Enlaces de interés
[Fernando Escolar / Observability](https://twitter.com/fernandoescolar/status/1712876195472200002?s=20)
[Manual de OpenTelemetry](https://opentelemetry.io/docs/concepts/instrumentation/manual/)
[GitHub - OpenTelemetry-DotNet](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Api/README.md)
[Convenciones de Telemetria](https://github.com/open-telemetry/semantic-conventions)