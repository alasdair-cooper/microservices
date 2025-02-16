namespace AlasdairCooper.Microservices.Dashboard

open Microsoft.Extensions.DependencyInjection

[<AutoOpen>]
type AppServices =
    { Services: IServiceCollection }
    
    static member Instance = { Services = ServiceCollection() }
    
    static member Build() = AppServices.Instance.Services.BuildServiceProvider()
