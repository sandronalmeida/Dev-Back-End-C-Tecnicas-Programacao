using Projeto_Final.Domain.Entities;

namespace Projeto_Final.Domain.Interface
{
    public interface ISmartNetService
    {
        bool ReservarIp(string mac, string ip);
        void AdicionarAFila(Dispositivo dispositivo);
        void ProcessarFila();
        void ExibirDashboard();
        void RelatorioFila();
        void RelatorioAtivos();
        void AplicarFirewall(Func<Dispositivo, bool> regraBloqueio);
        void DesfazerUltimaAcao();
    }
}
