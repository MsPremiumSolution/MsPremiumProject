using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Higrometria")]
public partial class Higrometria
{
    [Key]
    public ulong Id { get; set; }
    public decimal? HumidadeRelativaExterior { get; set; }
    public decimal? TemperaturaExterior { get; set; }
    public decimal? HumidadeRelativaInterior { get; set; }
    public decimal? TemperaturaInterior { get; set; }
    public decimal? TemperaturaParedesInternas { get; set; }
    public decimal? TemperaturaPontoOrvalho { get; set; }
    public decimal? PontoDeOrvalho { get; set; }
    [Column("pontos_frios")]
    public decimal? PontosFrios { get; set; }
    public decimal? NivelCO2 { get; set; }
    public decimal? NivelTCOV { get; set; }
    public decimal? NivelHCHO { get; set; }
    public decimal? DataLoggerSensores { get; set; }

    public virtual DadosGerais? DadosGerais { get; set; }
}