using Microsoft.Extensions.DependencyInjection;
using Projeto_Final.Application.Services;
using Projeto_Final.Domain.Entities;
using Projeto_Final.Domain.Interface;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();

        // Registro da dependência
        services.AddSingleton<ISmartNetService, SmartNetService>();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ISmartNetService>();

        bool executar = true;

        while (executar)
        {
            Console.WriteLine("\n=== SMARTNET MANAGER ===");
            Console.WriteLine("1 - Reservar IP");
            Console.WriteLine("2 - Adicionar dispositivo a fila");
            Console.WriteLine("3 - Processar fila");
            Console.WriteLine("4 - Exibir Dashboard");
            Console.WriteLine("5 - Relatório da Fila");
            Console.WriteLine("6 - Relatório Ativos");
            Console.WriteLine("7 - Firewall Apple");
            Console.WriteLine("8 - Firewall Consumo > 100 Mbps");
            Console.WriteLine("9 - Desfazer Última ação");
            Console.WriteLine("0 - Sair");

            Console.Write("Opção: ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("MAC: ");
                    var mac = Console.ReadLine();

                    Console.Write("IP: ");
                    var ip = Console.ReadLine();

                    Console.WriteLine(
                        service.ReservarIp(mac, ip)
                            ? "IP reservado."
                            : "MAC já cadastrado."
                    );
                    break;

                case "2":
                    Console.WriteLine("Digite M para incluir manualmente ou A para incluir 100 dispositivos automaticamente");
                    var inc = Console.ReadLine()?.ToUpper();

                    if (inc == "A")
                    {
                        var random = new Random();
                        var fabricantes = new[]
                        {
                            "Apple", "Dell", "Samsung", "Lenovo",
                            "HP", "Positivo", "Acer", "LG", "Asus"
                        };

                        for (int i = 1; i <= 100; i++)
                        {
                            var dispositivo = new Dispositivo
                            {
                                MacAddress = Guid.NewGuid().ToString(),
                                Nome = $"Dispositivo-{i}",
                                Fabricante = fabricantes[random.Next(fabricantes.Length)],
                                ConsumoBanda = random.Next(10, 800),
                                EnderecoIP = $"192.168.0.{random.Next(2, 250)}"
                            };

                            service.AdicionarAFila(dispositivo);
                        }

                        Console.WriteLine("100 dispositivos adicionados a fila de autentica��o.");
                    }
                    else if (inc == "M")
                    {
                        var dispositivo = new Dispositivo();

                        Console.Write("MAC: ");
                        dispositivo.MacAddress = Console.ReadLine();

                        Console.Write("Nome: ");
                        dispositivo.Nome = Console.ReadLine();

                        Console.Write("Fabricante: ");
                        dispositivo.Fabricante = Console.ReadLine();

                        Console.Write("Consumo Banda (Mbps): ");
                        if (double.TryParse(Console.ReadLine(), out double consumo))
                        {
                            dispositivo.ConsumoBanda = consumo;
                        }
                        else
                        {
                            Console.WriteLine("Valor inválido. Consumo definido como 0.");
                            dispositivo.ConsumoBanda = 0;
                        }

                        Console.Write("Endere�o IP: ");
                        dispositivo.EnderecoIP = Console.ReadLine();

                        service.AdicionarAFila(dispositivo);
                        Console.WriteLine("Dispositivo adicionado a fila.");
                    }
                    break;

                case "3":
                    service.ProcessarFila();
                    break;

                case "4":
                    service.ExibirDashboard();
                    break;

                case "5":
                    service.RelatorioFila();
                    break;

                case "6":
                    service.RelatorioAtivos();
                    break;

                case "7":
                    service.AplicarFirewall(d => d.Fabricante == "Apple");
                    Console.WriteLine("Firewall aplicado por fabricante.");
                    break;

                case "8":
                    service.AplicarFirewall(d => d.ConsumoBanda > 100);
                    Console.WriteLine("Firewall aplicado por consumo.");
                    break;

                case "9":
                    service.DesfazerUltimaAcao();
                    break;

                case "0":
                    executar = false;
                    break;
            }
        }
    }
}