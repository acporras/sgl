using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Globalization;

namespace SGL.Controllers
{
    public static class Uti
    {
        // Utility
        // Utilitarios

        public static string mod_region(string destino)
        {
            if (destino.Contains("HUACHO") || destino.Contains("LIMA") || destino.Contains("DIRSAL")) return "CAPITAL";
            else return "PROVINCIA";
        }

        public static DateTime mod_fecha(string fecha)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("es-ES");
            DateTime fecha_mod = DateTime.Parse(fecha, culture);
            return fecha_mod;
        }

        public static string mod_monto(string monto)
        {
            string nuevo_monto = monto;
            int size = monto.Length;
            int i = 0;
            char p = '.';
            char c = ',';
            foreach (char f in monto)
            {
                if (f.Equals(c))
                {
                    nuevo_monto = nuevo_monto.Substring(0, i) + "" + nuevo_monto.Substring(i + 1, size - i - 1);
                    i--;
                }
                i++;
            }
            i = 0;
            monto = nuevo_monto;
            size = monto.Length;
            foreach (char f in monto)
            {
                if (f.Equals(p)) nuevo_monto = nuevo_monto.Substring(0, i) + "," + nuevo_monto.Substring(i + 1, size - i - 1);
                i++;
            }
            return nuevo_monto;
        }

        public static string mod_octanaje(string octanaje)
        // Devuelve el octanaje en base a un diccionario.
        {
            try
            {
                return octanajes[octanaje];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                return octanaje;
            }

        }

        public static string mod_planta(string planta)
        // Devuelve la planta en base a un diccionario.
        {
            try
            {
                return plantas[planta];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                return planta;
            }

        }

        public static int GetNthIndex(string s, string t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].Equals(t))
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int FindNumberCharacters(string s, string t)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].Equals(t))
                {
                    count++;
                }
            }
            return count;
        }

        public static string mod_destino(string direccion, string planta, string regpol)
        // Devuelve el destino en base a un diccionario.
        {
            try
            {
                string nuevo = destinos[regpol];
                if (planta == "CONCHAN" && nuevo == "REGPOL LIMA")
                {
                    nuevo += " Y DIRECCIONES ESPECIALIZADAS";
                }
                else if (direccion.Contains("HUAURA") && (nuevo == "REGPOL LIMA"))
                {
                    nuevo += " JP. HUACHO";
                }
                else if ((planta == "CALLAO" && nuevo == "REGPOL LIMA"))
                {
                    nuevo += " CALLAO Y DIRECCIONES ESPECIALIZADAS";
                }

                if (nuevo.Contains("TUMBES")) nuevo = "REGPOL TUMBES";
                if (nuevo.Contains("LAMBAYEQUE")) nuevo = "REGPOL LAMBAYEQUE";
                if (nuevo.Contains("JUNIN")) nuevo = "REGPOL JUNIN";
                if (nuevo.Contains("AYACUCHO ")) nuevo = "REGPOL AYACUCHO";
                if (nuevo.Contains("VRAEM ")) nuevo = "FP VRAEM";

                return nuevo;
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                string nuevo = regpol;
                if (nuevo.Contains("TUMBES")) nuevo = "REGPOL TUMBES";
                if (nuevo.Contains("LAMBAYEQUE")) nuevo = "REGPOL LAMBAYEQUE";
                if (nuevo.Contains("JUNIN")) nuevo = "REGPOL JUNIN";
                if (nuevo.Contains("AYACUCHO ")) nuevo = "REGPOL AYACUCHO ";
                if (nuevo.Contains("VRAEM ")) nuevo = "FP VRAEM";
                return nuevo;
            }
        }

        public static string ultima_palabra(string planta)
        // Devuelve la última palabra de una cadena.
        // SIN USO.
        {
            int index = planta.Length - 1;
            int con = 0;
            string root2 = " ";
            for (int i = index; i > -1; i--)
            {
                string root1 = planta[i].ToString();
                if (root1.Equals(root2))
                {
                    string result = accent(planta.Substring(index + 1 - con, con));
                    return result;
                }
                con++;
            }
            return "";
        }

        public static string accent(string accentedStr)
        // Remueve los acentos;
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(accentedStr);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr;
        }

        static IDictionary<string, string> destinos = new Dictionary<string, string>()
            {
                {  "REGPOL LAMBAYEQUE - ESQ. CALLE MANCO CAPAC N 205 CHICLAYO Lambayeque CHICLAYO", "REGPOL LAMBAYEQUE"},
                {  "REGPOL TUMBES - AV. TUMBES NORTE N 1033 TUMBES Tumbes TUMBES", "REGPOL TUMBES"},
                {  "REGPOL JUNIN - JR. CUZCO N° 666 HUANCAYO Junín HUANCAYO", "REGPOL JUNIN"},
                {  "REGPOL AYACUCHO ASOC. UNSCH MZ E LT 5 AYACUCHO Ayacucho HUAMANGA", "REGPOL AYACUCHO"},
                {  "FP VRAEM AV HEROES DEL VRAEM S/N URB. PICHARI PICHARI Cuzco LA CONVENCION", "FP VRAEM"},
                {  "AV.BRASIL CDRA 26 -HOSPITAL LUIS  N.SAENZ JESUS MARIA Lima LIMA", "DIRSAL PNP"},
                {  "REGPOL UCAYALI AV. SAN MARTIN  N 466 CALLERIA Lima CORONEL PORTILLO", "REGPOL UCAYALI"},
                {  "REGPOL MOQUEGUA- CALLE AMAZONAS  Nº S/N MOQUEGUA Moq eg a MARISCAL NIETO",  "REGPOL MOQUEGUA"},
                {  "REGPOL LAMBAYEQUE - ESQ. CALLE  MANCO CAPAC N 205 CHICLAYO Lambayeq e CHICLAYO",  "REGPOL LAMBAYEQUE"},
                {  "REGPOL HUANUCO JR. CONSTITUCION  NRO. 501 CENT. U HUANUCO HUANUCO Huánuco HUANUCO",  "REGPOL HUANUCO"},
                {  "REGPOL AYACUCHO ASOC. UNSCH  MZ E LT 5 AYACUCHO Ayac cho HUAMANGA",  "REGPOL AYACUCHO"},
                {  "FP VRAEM  AV HEROES DEL VRAEM  S/N URB. PICHARI PICHARI C zco LA CONVENCION",  "CUP. VRAEM"},
                {  "REGPOL LA LIBERTAD -JR.  BOLOGNESI N 428 CENTRO CIVICO TRUJILLO La Libertad TRUJILLO",  "REGPOL LA LIBERTAD"},
                {  "REGPOL ANCASH - JR 28 DE JULIO  N 755 HUARAZ Ancash HUARAZ",  "REGPOL ANCASH"},
                {  "REGPOL  PIURA - KM 3.5 PANA.   NORTE ZONA INDUSTRIAL PIURA Pi ra PIURA",  "REGPOL PIURA"},
                {  "REGPOL  LIMA- CALLAO PJ. SAN  GERMAN NRO. CDR2  URB. VILLACAMPA RIMAC Lima LIMA",  "REGPOL LIMA"},
                {  "REGPOL TACNA - JR. CALDERON DE  LA BARCA N 353 - 357 TACNA Tacna TACNA",  "REGPOL TACNA"},
                {  "REGPOL LORETO AV. BRASIL N 145 IQUITOS Lima MAYNAS",  "REGPOL LORETO"},
                {  "REGPOL CUSCO - PLAZA TUPAC  AMARU S/N WANCHAQ Cuzco CUSCO",  "REGPOL CUSCO"},
                {  "REGPOL HUANCAVELICA - JR.  GRAU N S/N PLAZA RAMON CASTILLA HUANCAVELICA H ancavelica HUANCAVELICA",  "REGPOL HUANCAVELICA"},
                {  "AV. LOS PROCERES INTERSECCION  CON LA AV. LOPEZ ALBUJAR S/N URB. SAN JUAN PAMPA REGPOL PASCO YANACANCHA Pasco PASCO",  "REGPOL PASCO"},
                {  "REGPOL APURIMAC - JR LIMA N 1000 ABANCAY Apurimac ABANCAY",  "REGPOL APURIMAC"},
                {  "REGPOL JUNIN - JR. CUZCO  Nº 666 HUANCAYO Junín HUANCAYO",  "REGPOL JUNIN"},
                {  "REGPOL TUMBES  - AV. TUMBES  NORTE N 1033 TUMBES T mbes TUMBES",  "REGPOL TUMBES"},
                {  "JR. AMALIA PUGA N 1111 BARRIO  SAN SEBASTIAN - REGPOL CAJAMARCA CAJAMARCA Cajamarca CAJAMARCA",  "REGPOL CAJAMARCA"},
                {  "DIVPOL CHIMBOTE - CALLE LEONCIO  PRADO Nº401 CHIMBOTE Ancash SANTA",  "DIVPOL CHIMBOTE"},
                {  "REGPOL ICA - AV. JUAN J.ELIAS  5TA Y 6TA. CRA - URB. SAN ISIDRO ICA Ica ICA",  "REGPOL ICA"},
                {  "REGPOL AMAZONAS JR AYACUCHO  N 1040 CHACHAPOYAS Amazonas AMAZONAS",  "REGPOL AMAZONAS"},
                {  "REGPOL SAN MARTIN JR. RAMIREZ  HURTADO N 280 TARAPOTO San Martín SAN MARTIN",  "REGPOL SAN MARTIN"},
                {  "REGPOL  PUNO - AV. EL SOL N 450   - CERCADO PUNO PUNO Puno PUNO",  "REGPOL PUNO"},
                {  "REGPOL AREQUIPA AV. EMMEL N 106 YANAHUARA Areq ipa AREQUIPA",  "REGPOL AREQUIPA"},
                {  "REGPOL AREQUIPA AV. EMMEL N 106 YANAHUARA Arequipa AREQUIPA", "REGPOL AREQUIPA" },
                {  "ESQ. JR. LORETO Y PUNO  REGPOL MADRE DE DIOS TAMBOPATA Lima TAMBOPATA",  "REGPOL MADRE DE DIOS"}, 
            };

        static IDictionary<string, string> plantas = new Dictionary<string, string>()
            {
                {"4002 PLANTA DE VENTAS EL MILAGRO", "EL MILAGRO"},
                { "2004 PLANTA DE VENTAS YURIMAGUAS", "YURIMAGUAS"},
                {  "2003 PLANTA DE VENTAS PUCALLPA", "PUCALLPA"},
                {  "5101 TERMINAL CHIMBOTE",  "CHIMBOTE"},
                {  "4002 Planta de Ventas  El Milagro",  "EL MILAGRO"},
                {  "1002 PLANTA VENTAS PIURA",  "PIURA"},
                {  "5302 TERMINAL MOLLENDO",  "MOLLENDO"},
                {  "5203 TERMINAL SUPE",  "SUPE"},
                {  "5202 TERMINAL PISCO",  "PISCO"},
                {  "3001 PLANTA DE VENTAS CONCHÁN",  "CONCHAN"},
                {  "5102 TERMINAL ETEN",  "ETEN"},
                {  "5303 PLANTA CUSCO",  "CUSCO"},
                {  "5306 OFIC. VENTAS PUERTO MALDONADO",  "PTO. MALDONADO"},
                {  "1001 PLANTA VENTAS TALARA",  "TALARA"},
                {  "5304 PLANTA JULIACA",  "JULIACA"},
                {  "5301 TERMINAL ILO",  "ILO"},
                {  "5103 TERMINAL SALAVERRY",  "SALAVERRY"},
                {  "5204 PLANTA VILLA DE PASCO",  "C. DE PASCO"},
                {  "2005 PLANTA DE VENTAS TARAPOTO",  "TARAPOTO"},
                {  "5201 TERMINAL CALLAO",  "CALLAO"},
                {  "2001 PLANTA VENTA IQUITOS",  "IQUITOS"}
            };

        static IDictionary<string, string> octanajes = new Dictionary<string, string>()
            {
                { "GASOHOL SUPER PLUS 95",  "GASOHOL SP 95"},
                {  "GASOHOL 97 PLUS E", "GASOHOL 97 PE"},
                {  "GASOLINA SUPER PLUS 84", "GASOLINA SP 84"},
                {  "GASOHOL SUPER PLUS 97",  "GASOHOL SP 97"},
                {  "GASOHOL SUPER PLUS 90",  "GASOHOL SP 90"},
                {  "DIESEL B5 PETROPERU",  "DIESEL B5"},
                {  "GASOLINA SUPER PLUS 90",  "GASOLINA SP 90"},
                {  "GASOHOL 95 PLUS E",  "GASOHOL 95 PE"},
                {  "GASOHOL 90 PLUS E",  "GASOHOL 90 PE"},
                {  "DIESEL B5 S-50 PETROPERU",  "DIESEL B5 S-50"},
                {  "GASOHOL 95 PLUS",  "GASOHOL 95 P"},
                {  "GASOLINA SUPER 90 SP E",  "GASOLINA S 90 SP E"},
                {  "GASOHOL SUPER PLUS 84",  "GASOHOL SP 84"}
            };

    }
}