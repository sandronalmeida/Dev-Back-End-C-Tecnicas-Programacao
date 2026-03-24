using Projeto_Final.Domain.Entities;
using Projeto_Final.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_Final.Application.Services
{
    public class SmartNetService : ISmartNetService
    {
        private readonly Dictionary<string, string> _reservasIp = new();
        private readonly Queue<Dispositivo> _filaAutenticacao = new();
        private readonly List<Dispositivo> _dispositivosAtivos = new();
        private readonly Stack<string> _historicoAuditoria = new();

        public bool ReservarIp(string mac, string ip)
        {
            if (_reservasIp.ContainsKey(mac))
                return false;

            _reservasIp.Add(mac, ip);
            return true;
        }

        public void AdicionarAFila(Dispositivo dispositivo)
            => _filaAutenticacao.Enqueue(dispositivo);

        public void ProcessarFila()
        {
            if (!_filaAutenticacao.Any())
            {
                Console.WriteLine("Fila vazia.");
                return;
            }

            while (_filaAutenticacao.Any())
            {
                var dispositivo = _filaAutenticacao.Dequeue();
                dispositivo.DataConexao = DateTime.Now;

                _dispositivosAtivos.Add(dispositivo);
                _historicoAuditoria.Push($"Dispositivo autenticado: {dispositivo.Nome}");
            }
        }

        public void ExibirDashboard()
        {
            Console.WriteLine("=== DASHBOARD ===");

            var consumoTotal = _dispositivosAtivos.Sum(d => d.ConsumoBanda);
            Console.WriteLine($"Consumo total: {consumoTotal} Mbps");

            Console.WriteLine("\nTop 3 consumo:");
            foreach (var d in _dispositivosAtivos
                .OrderByDescending(d => d.ConsumoBanda)
                .Take(3))
                Console.WriteLine($"{d.Nome} - {d.ConsumoBanda} Mbps");

            Console.WriteLine("\nFabricantes:");
            foreach (var grupo in _dispositivosAtivos.GroupBy(d => d.Fabricante))
                Console.WriteLine($"{grupo.Key}: {grupo.Count()}");

            if (_dispositivosAtivos.Any(d => d.ConsumoBanda > 500))
                Console.WriteLine("\nALERTA: Consumo anormal!");

            Console.WriteLine("\nAuditoria:");
            foreach (var h in _historicoAuditoria)
                Console.WriteLine(h);
            Console.WriteLine("\n######################################");
        }

        public void RelatorioFila()
            => ImprimirRelatorio(_filaAutenticacao);

        public void RelatorioAtivos()
            => ImprimirRelatorio(_dispositivosAtivos);

        private static void ImprimirRelatorio(IEnumerable<Dispositivo> colecao)
        {
            foreach (var d in colecao)
                Console.WriteLine($"{d.Nome} | {d.Fabricante} | {d.ConsumoBanda} Mbps");
            Console.WriteLine("\n######################################");
        }


        public void AplicarFirewall(Func<Dispositivo, bool> regraBloqueio)
        {
            var bloqueados = _dispositivosAtivos.Where(regraBloqueio).ToList();

            foreach (var d in bloqueados)
            {
                _dispositivosAtivos.Remove(d);
                _historicoAuditoria.Push($"Firewall no dispositivo: {d.Nome}");
            }
            Console.WriteLine("\n######################################");
        }

        public void DesfazerUltimaAcao()
        {
            if (!_historicoAuditoria.Any())
            {
                Console.WriteLine("Nada para desfazer.");
                return;
            }

            Console.WriteLine($"Desfeito: {_historicoAuditoria.Pop()}");
        }
    }
}
