// File: Utils/EuropeanNifValidator.cs
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// using Microsoft.Extensions.Logging; // Descomenta se quiseres usar ILogger aqui

namespace MSPremiumProject.Utils
{
    public static class EuropeanNifValidator
    {
        // Se quiseres logar avisos (ex: país não suportado), podes injetar ILogger via construtor
        // ou passá-lo como parâmetro, mas para uma classe estática é mais complexo.
        // Para simplicidade, o logging de "país não suportado" pode ser feito pelo chamador.

        private static readonly Dictionary<string, string> NifRegexPatterns = new Dictionary<string, string>
        {
            // Portugal
            { "PT", @"^(1|2|3|5|6|8|9)\d{8}$" },
            // Espanha (NIF, NIE e CIF)
            { "ES", @"^([A-Z]\d{7}[A-Z0-9]|[0-9]{8}[A-Z])$" },
            // França (numéro de TVA intracommunautaire)
            { "FR", @"^[0-9A-Z]{2}\d{9}$" },
            // Alemanha
            { "DE", @"^\d{9}$" },
            // Itália (Partita IVA - 11 dígitos numéricos)
            // Se for Codice Fiscale, o regex é diferente: @"^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$"
            { "IT", @"^\d{11}$" }, // Assumindo Partita IVA. Ajusta se for Codice Fiscale.
            // Bélgica
            { "BE", @"^(0|1)?\d{9}$" },
            // Países Baixos (Holanda) - btw-nummer
            { "NL", @"^\d{9}B\d{2}$" },
            // Reino Unido (VAT number)
            { "GB", @"^((GD|HA)\d{3}|\d{9}|\d{12})$" },
            // Polónia
            { "PL", @"^\d{10}$" },
            // Grécia (AFM) - EL é o prefixo VAT, GR é o código ISO do país
            { "GR", @"^\d{9}$" }, // Usando GR para código de país consistente. Ajusta se usares EL.
            // Suécia (Organisationsnummer/Personnummer)
            { "SE", @"^(\d{6}|\d{8})[-\s]?\d{4}$" }
            // Adiciona outros países e os seus regex aqui
        };

        public static bool ValidateNif(string countryCode, string nif)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(nif))
            {
                return false;
            }

            countryCode = countryCode.ToUpper().Trim();
            nif = Regex.Replace(nif.ToUpper().Trim(), @"[\s\-\.]", ""); // Limpa espaços, hífens, pontos

            if (!NifRegexPatterns.TryGetValue(countryCode, out string? pattern))
            {
                // Código de país não encontrado no nosso dicionário.
                // Em vez de lançar exceção, retornamos false.
                // O chamador pode decidir como lidar com isto (ex: logar, mostrar erro específico).
                return false;
            }

            if (string.IsNullOrEmpty(pattern)) // Verificação de segurança, não deve acontecer.
            {
                return false;
            }

            return Regex.IsMatch(nif, pattern);
        }
    }
}