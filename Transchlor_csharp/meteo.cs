using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic;

static class Meteo
{
    private static int DataLength = 440000;
    private static StrctPanne[] arrPanne = new StrctPanne[DataLength + 1]; // matrice d'analyse des pannes, conçu pour 50ans mesure chaque heure
    private static StrctCalc[] arrMatrice = new StrctCalc[DataLength + 1]; // matrice de calcul, conçu pour 50ans mesure chaque heure
    private static StrctMeteo[] arrDaten = new StrctMeteo[DataLength + 1]; // matrice input météo, conçu pour 50ans mesure chaque heure
    // Dim frmTempSeuil As frmMeteo
    private static int iAnzahl; // nombre de ligne
    private static double NbrAns;
    private static string Export;
    private static short CasInput;

    struct StrctMeteo // colonnes de la matrice à partir du fichier METEO_*.txt
    {
        public int datum; // date YYYYMMDD
        public int heure; // heure HH
        public float moy6; // température [0.1°C]
        public float moy13; // humidité relative [0/00]
        public long moy17; // h pluie [0.1mm]
        public float moy22; // rayonnement globale [Wh/m2]
        public float moy80; // h neige neuve [mm]
        public float neige;  // h neige calculé
    }

    public struct StrctForm // colonnes de la matrice de calcul
    {
        public float nb_annees; // nombre d'années
        public float concentration_annuelle_chlorure_sodium_epandage_mecanique; // concentration annuelle de chlorure de sodium epandage mécanique
        public float concentration_annuelle_chlorure_sodium_epandage_automatique; // concentration annuelle de chlorure de sodium epandage automatique
        public float temperature_seuil_epandage_mecanique; // température seuil epandage mécanique
        public float temperature_seuil_epandage_automatique; // température seuil epandage automatique
        public float nb_intervention_epandage; // nombre d'intervention d'épandage
        public float nb_giclages_annuel; // nombre de giclages annuel

        public float concentration_chlorure; // concentration de chlorure de sodium
        public float epaisseur_film_eau_chaussee; // épaisseur du film d'eau sur la chaussée
        public float humidite_relative_seuil_intervention; // humidité relative seuil d'intervention

        public float quantite_moyenne_chlorure_epandage_mecanique; // quantité moyenne de chlorure de sodium epandage mécanique
        public float intervalle_minimal_entre_2; // intervalle minimal entre 2
        public float concentration_chlorure_sodium_epandage_mecanique; // concentration de chlorure de sodium epandage mécanique
        public float quantite_moyenne_chlorure_epandage_automatique; // quantité moyenne de chlorure de sodium epandage automatique
        public float nb_giclage_par_intervalle; // nombre de giclage par intervalle
        public float concentration_chlorure_sodium_epandage_automatique; // concentration de chlorure de sodium epandage automatique

        public float position_de_la_1_temperature_exterieur; // position de la 1 température extérieure
        public float position_de_la_2_temperature_exterieur; // position de la 2 température extérieure
        public float attenuation_de_1_temperature_exterieur; // atténuation de 1 température extérieure
        public float attenuation_de_2_temperature_exterieur; // atténuation de 2 température extérieure
        public float difference_de_temperature_exterieur; // différence de temperature extérieure

        public float position_de_la_1_humidite_exterieur; // position de la 1 humidité extérieure
        public float position_de_la_2_humidite_exterieur; // position de la 2 humidité extérieure
        public float attenuation_de_1_humidite_exterieur; // atténuation de 1 humidité extérieure
        public float attenuation_de_2_humidite_exterieur; // atténuation de 2 humidité extérieure
        public float difference_de_humidite_exterieur; // différence de humidité extérieure

        public float position_de_la_1_temperature_interieure; // position de la 1 température intérieure
        public float position_de_la_2_temperature_interieure; // position de la 2 température intérieure
        public float attenuation_de_1_temperature_interieure; // atténuation de 1 température intérieure
        public float attenuation_de_2_temperature_interieure; // atténuation de 2 température intérieure
        public float difference_de_temperature_interieure; // différence de temperature intérieure

        public float position_de_la_1_humidite_interieure; // position de la 1 humidité intérieure
        public float position_de_la_2_humidite_interieure; // position de la 2 humidité intérieure
        public float attenuation_de_1_humidite_interieure; // atténuation de 1 humidité intérieure
        public float attenuation_de_2_humidite_interieure; // atténuation de 2 humidité intérieure
        public float difference_de_humidite_interieure; // différence de humidité intérieure
    }

    struct StrctCalc // colonnes de la matrice de calcul
    {
        public int year1; // année YYYY
        public int month; // mois MM
        public int day; // jour DD
        public int hour; // heure HH
        public float year2; // année en décimale YYYY,....
        public float HR_brouillard; // exposition brouillard [%]
        public float HR_eclaboussures; // exposition eclaboussures [%]
        public float HR_direct; // exposition directe [%]
        public float HR_ext; // exposition à l'extérieur à l'abri des précipitations [%]
        public float HR_caisson; // exposition dans les caissons [%]
        public float HR_bitume; // exposition dans les caissons [%]
        public string salage1; // salage mécanique
        public string salage2; // salage automatique
        public float T; // température air ventilée [°C]
        public float Ts; // température de surface équivalente [°C]
        public float Tcaisson;   // température à l'intérieur caisson [°C]
        public float Text;   // température extérieure, à l'abri des précipitations [°C]
    }

    struct StrctPanne // colonnes de la matrice des pannes
    {
        public int PanneStart; // colonnes début des pannes
        public int PanneEnd; // colonnes fin des pannes
        public string PanneMesure; // colonnes des types de pannes
    }

    struct Meteo_File // fichier INPUT
    {
        public float HR; // colonnes HR
        public float Sel; // colonnes salage
        public float Tsurf; // colonnes Température de surface (T ou Ts)

        internal static void WriteAllBytes(string savePath, byte[] fileData)
        {
            throw new NotImplementedException();
        }
    }

    public static void SetExport(ref string Value)
    {
        Export = Value;
    }

    public static void FilePost(ref string outfile, ref string PostFile)
    {
        short iPos;
        string Dim1;
        short iPos_old = 1;

        iPos = 10;
        Dim1 = "\\";
        while (iPos > 0)
        {
            iPos = (short)Strings.InStr(iPos_old, outfile, Dim1, CompareMethod.Text);
            if (iPos != 0) iPos_old = (short)(iPos + 1);
        }
        iPos = (short)(iPos_old - 1);
        PostFile = Strings.Left(outfile, iPos);
        Directory.SetCurrentDirectory(PostFile);
    }

    public static void FileOnly(ref string outfile)
    {
        short iPos;
        string Dim1;

        iPos = 10;
        Dim1 = "\\";
        while (iPos > 0)
        {
            iPos = (short)Strings.InStr(1, outfile, Dim1, CompareMethod.Text);
            if (iPos != 0)
            {
                iPos = (short)(outfile.Length - iPos);
                outfile = Strings.Right(outfile, iPos);
            }
        }
    }


    public static void ReadMeteoFile(string OutFile, ref string PostFile, ref string txtFile, ref bool Canc)
    {
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // lecture fichier METEO_*.txt
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        var Filtre = "Text files (METEO_*.txt)|METEO_*.txt";
        var Index = 1;
        var Directoire = true;
        var Titre = "Sélectionner le fichier météo";

        // OpenDialog(OutFile, Canc, Filtre, Index, Directoire, Titre)
        // If Canc = True Then End

        int nFic = Microsoft.VisualBasic.FileSystem.FreeFile();

        using (StreamReader sr = new StreamReader(OutFile))
        {
            FilePost(ref OutFile, ref PostFile);
            FileOnly(ref OutFile);
            int posTxt;
            posTxt = OutFile.Length - 10;
            txtFile = OutFile.Substring(7, posTxt);

            string line;
            line = sr.ReadLine(); // ligne 1 fait rien. Nom d

            line = sr.ReadLine(); // ligne 2 donne le nombre de lignes

            try
            {
                DataLength = System.Convert.ToInt32(line);
                arrPanne = new StrctPanne[DataLength + 1];
                arrMatrice = new StrctCalc[DataLength + 1];
                arrDaten = new StrctMeteo[DataLength + 1];
            }
            catch
            {
            }

            line = sr.ReadLine(); // lire linge 3

            int MyPos6 = line.IndexOf("6"); // recherche des titre des colonnes 
            int MyPos13 = line.IndexOf("13");
            int MyPos17 = line.IndexOf("17");
            int MyPos22 = line.IndexOf("22");
            int MyPos80 = line.IndexOf("80");

            if (MyPos80 != 0)
                CasInput = 1;// matriceStrctMeteo avec les colonnes 6,13,17,22,80
            if (MyPos80 == 0)
                CasInput = 2;// matriceStrctMeteo avec les colonnes 6,13,17,22 (sans neige)

            bool bFertig = false;
            int i = 0;
            while (!bFertig)
            {
                try // test s'il y a du text ou pas
                {
                    arrDaten[i].datum = int.Parse(sr.ReadLine());
                }
                catch
                {
                    bFertig = true;
                }

                if (!bFertig)
                {
                    // les quatre cas:
                    if (CasInput == 1)
                    {
                        arrDaten[i].heure = int.Parse(sr.ReadLine());
                        arrDaten[i].moy6 = float.Parse(sr.ReadLine());
                        arrDaten[i].moy13 = float.Parse(sr.ReadLine());
                        arrDaten[i].moy17 = long.Parse(sr.ReadLine());
                        arrDaten[i].moy22 = float.Parse(sr.ReadLine());
                        arrDaten[i].moy80 = float.Parse(sr.ReadLine());
                    }
                    else if (CasInput == 2)
                    {
                        arrDaten[i].heure = int.Parse(sr.ReadLine());
                        arrDaten[i].moy6 = float.Parse(sr.ReadLine());
                        arrDaten[i].moy13 = float.Parse(sr.ReadLine());
                        arrDaten[i].moy17 = long.Parse(sr.ReadLine());
                        arrDaten[i].moy22 = float.Parse(sr.ReadLine());
                    }

                    if ((arrDaten[i].datum - ((Conversion.Fix(arrDaten[i].datum / (double)10000)) * 10000) != 229))
                        i = i + 1;// élimination du 29. février
                }
            }

            iAnzahl = i;
        }

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // calcul des dates dans la matrice
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        for (var i = 0; i <= (iAnzahl - 1); i++)
        {
            arrMatrice[i].year1 = (int)Conversion.Fix(arrDaten[i].datum / (double)10000);
            arrMatrice[i].year2 = (float)(arrMatrice[i].year1 + arrMatrice[i].month / (double)12 + arrMatrice[i].day / (double)366 + arrMatrice[i].hour / (double)(24 * 366));
            arrMatrice[i].month = (int)Conversion.Fix((arrDaten[i].datum - 10000 * arrMatrice[i].year1) / (double)100);
            arrMatrice[i].day = arrDaten[i].datum - arrMatrice[i].year1 * 10000 - arrMatrice[i].month * 100;
            arrMatrice[i].hour = arrDaten[i].heure;
            arrMatrice[i].year2 = (float)(arrMatrice[i].year1 + arrMatrice[i].month / (double)12 + arrMatrice[i].day / (double)366 + arrMatrice[i].hour / (double)(24 * 366));
        }
    }

    public static string Troubleshoot(Int32 number)
    {

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // détection des pannes
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        bool Panne = false;
        int NbrPanne = 0;
        int i = 0;

        for (i = 0; i <= (iAnzahl - 1); i++)
        {
            if (arrDaten[i].moy6 == 32767 & !Panne)
            {
                Panne = true;
                arrPanne[NbrPanne].PanneStart = i;
                arrPanne[NbrPanne].PanneMesure = "mm de pluie";
            }
            if (i == iAnzahl - 1 & Panne == true)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i;
                NbrPanne = NbrPanne + 1;
            }
            if (arrDaten[i].moy6 != 32767 & Panne)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i - 1;
                NbrPanne = NbrPanne + 1;
            }
        }

        for (i = 0; i <= (iAnzahl - 1); i++)
        {
            if (arrDaten[i].moy13 == 32767 & !Panne)
            {
                Panne = true;
                arrPanne[NbrPanne].PanneStart = i;
                arrPanne[NbrPanne].PanneMesure = "Température";
            }
            if (i == iAnzahl - 1 & Panne == true)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i;
                NbrPanne = NbrPanne + 1;
            }
            if (arrDaten[i].moy13 != 32767 & Panne)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i - 1;
                NbrPanne = NbrPanne + 1;
            }
        }

        for (i = 0; i <= (iAnzahl - 1); i++)
        {
            if (arrDaten[i].moy17 == 32767 & !Panne)
            {
                Panne = true;
                arrPanne[NbrPanne].PanneStart = i;
                arrPanne[NbrPanne].PanneMesure = "Humidité relaltive";
            }
            if (i == iAnzahl - 1 & Panne == true)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i;
                NbrPanne = NbrPanne + 1;
            }
            if (arrDaten[i].moy17 != 32767 & Panne)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i - 1;
                NbrPanne = NbrPanne + 1;
            }
        }

        for (i = 0; i <= (iAnzahl - 1); i++)
        {
            if (arrDaten[i].moy22 == 32767 & !Panne)
            {
                Panne = true;
                arrPanne[NbrPanne].PanneStart = i;
                arrPanne[NbrPanne].PanneMesure = "Rayonnement globale";
            }
            if (i == iAnzahl - 1 & Panne == true)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i;
                NbrPanne = NbrPanne + 1;
            }
            if (arrDaten[i].moy22 != 32767 & Panne)
            {
                Panne = false;
                arrPanne[NbrPanne].PanneEnd = i - 1;
                NbrPanne = NbrPanne + 1;
            }
        }

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // afficher les pannes
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        var MessagePanne = "";
        string strDebut;
        string strFin;

        for (i = 0; i <= NbrPanne - 1; i++)
        {
            strDebut = arrMatrice[arrPanne[i].PanneStart].day + "." + arrMatrice[arrPanne[i].PanneStart].month + "." + arrMatrice[arrPanne[i].PanneStart].year1 + " à " + arrMatrice[arrPanne[i].PanneStart].hour + "H  ";
            strFin = arrMatrice[arrPanne[i].PanneEnd].day + ". " + arrMatrice[arrPanne[i].PanneEnd].month + "." + arrMatrice[arrPanne[i].PanneEnd].year1 + " à " + arrMatrice[arrPanne[i].PanneEnd].hour + "H  ";
            MessagePanne = MessagePanne + "Panne " + (i + 1) + "  du  " + strDebut + "  au  " + strFin + arrPanne[i].PanneMesure + (Constants.vbCrLf);
        }

        if (NbrPanne == 0)
            MessagePanne = "Il n'y a pas de panne dans la série!";
        // ' MsgBox(MessagePanne, , "Détéction des pannes")
        string MessageFinalePanne;
        MessageFinalePanne = MessagePanne + " " + "Détéction des pannes";

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // recherche de l'intervalle le plus long sans pannes
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        int Start;
        int Fin;
        int startmax;
        int finmax;
        int IntLong;
        int IntrStart;

        Start = 0;
        Fin = 0;
        startmax = 0;
        finmax = 0;
        Panne = false;
        for (i = 0; i <= iAnzahl - 1; i++) // i correspond à une heure
        {
            if (arrDaten[i].moy6 == 32767 | arrDaten[i].moy13 == 32767 | arrDaten[i].moy17 == 32767 | arrDaten[i].moy22 == 32767)
            {
                if (Panne == false)
                {
                    Fin = i - 1;
                    if (Fin - Start > finmax - startmax)
                    {
                        finmax = Fin;
                        startmax = Start;
                    }
                    Panne = true;
                }
            }
            else if (Panne == true)
            {
                Start = i;
                Panne = false;
            }
        }

        if (Panne == false)
        {
            Fin = i - 1;
            if (Fin - Start > finmax - startmax)
            {
                finmax = Fin;
                startmax = Start;
            }
        }

        IntLong = finmax - startmax;
        // IntLong = Fix(IntLong / 8760)
        // iAnzahl = CInt(8760 * IntLong)
        iAnzahl = System.Convert.ToInt32(IntLong);

        strDebut = arrMatrice[startmax].day + "." + arrMatrice[startmax].month + "." + arrMatrice[startmax].year1 + " à " + arrMatrice[startmax].hour + "H  ";
        strFin = arrMatrice[startmax + iAnzahl - 1].day + "." + arrMatrice[startmax + iAnzahl - 1].month + "." + arrMatrice[startmax + iAnzahl - 1].year1 + " à " + arrMatrice[startmax + iAnzahl - 1].hour + "H  ";
        // 'MsgBox(" du  " & strDebut & " au  " & strFin, , "Interval maximal sans pannes ")
        string MessageFinaleInterval;
        MessageFinaleInterval = " du  " + strDebut + " au  " + strFin + " " + "Interval maximal sans pannes ";

        NbrAns = iAnzahl / (double)8760;

        for (i = 0; i <= iAnzahl - 1; i++)
            arrMatrice[i] = arrMatrice[i + startmax];
        var oldArrMatrice = arrMatrice;
        arrMatrice = new StrctCalc[iAnzahl - 1 + 1];
        if (oldArrMatrice != null)
            Array.Copy(oldArrMatrice, arrMatrice, Math.Min(iAnzahl - 1 + 1, oldArrMatrice.Length));
        for (i = 0; i <= iAnzahl - 1; i++)
            arrDaten[i] = arrDaten[i + startmax];
        var oldArrDaten = arrDaten;
        arrDaten = new StrctMeteo[iAnzahl - 1 + 1];
        if (oldArrDaten != null)
            Array.Copy(oldArrDaten, arrDaten, Math.Min(iAnzahl - 1 + 1, oldArrDaten.Length));

        if (number == 0)
            return MessageFinalePanne;
        else
            return MessageFinaleInterval;
    }

    public static StrctForm precalcul(string outfile, StrctForm form)
    {
        var PostFile = "";
        var txtfile = "";
        bool Canc = false;

        ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);


        int Start;
        int Fin;
        int startmax;
        int finmax;
        int IntLong;
        bool Panne = false;

        Start = 0;
        Fin = 0;
        startmax = 0;
        finmax = 0;
        Panne = false;
        int i = 0;
        for (i = 0; i <= iAnzahl - 1; i++) // i correspond à une heure
        {
            if (arrDaten[i].moy6 == 32767 | arrDaten[i].moy13 == 32767 | arrDaten[i].moy17 == 32767 | arrDaten[i].moy22 == 32767)
            {
                if (Panne == false)
                {
                    Fin = i - 1;
                    if (Fin - Start > finmax - startmax)
                    {
                        finmax = Fin;
                        startmax = Start;
                    }
                    Panne = true;
                }
            }
            else if (Panne == true)
            {
                Start = i;
                Panne = false;
            }
        }

        if (Panne == false)
        {
            Fin = i - 1;
            if (Fin - Start > finmax - startmax)
            {
                finmax = Fin;
                startmax = Start;
            }
        }

        IntLong = finmax - startmax;
        // IntLong = Fix(IntLong / 8760)
        // iAnzahl = CInt(8760 * IntLong)
        iAnzahl = System.Convert.ToInt32(IntLong);
        NbrAns = iAnzahl / (double)8760;
        var oldArrMatrice = arrMatrice;
        arrMatrice = new StrctCalc[iAnzahl - 1 + 1];
        if (oldArrMatrice != null)
            Array.Copy(oldArrMatrice, arrMatrice, Math.Min(iAnzahl - 1 + 1, oldArrMatrice.Length));
        for (i = 0; i <= iAnzahl - 1; i++)
            arrDaten[i] = arrDaten[i + startmax];
        var oldArrDaten = arrDaten;
        arrDaten = new StrctMeteo[iAnzahl - 1 + 1];
        if (oldArrDaten != null)
            Array.Copy(oldArrDaten, arrDaten, Math.Min(iAnzahl - 1 + 1, oldArrDaten.Length));




        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // Calcul du nbre d'interventions et de la quantité de sel épandu
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        bool Hiv = true;
        var Cpt = 0;
        float NDH = 0;



        // Calcul du nombre de jours hivernaux
        for (int j = 0; j <= iAnzahl - 1; j++)
        {
            if (arrDaten[j].moy6 / (double)10 > 0)
                Hiv = false;
            if (Cpt == 24)
            {
                if (Hiv == true)
                    NDH = NDH + 1;
                Hiv = true;
                Cpt = 0;
            }
            Cpt = Cpt + 1;
        }

        // frmTempSeuil = New frmMeteo
        // frmTempSeuil.Label12.Text = NbrAns

        form.nb_annees = (float)NbrAns;

        if (Math.Round(NbrAns, 1) > Math.Round(NbrAns, 0))
            NbrAns = Math.Round(NbrAns, 0) + 1;
        else
            NbrAns = Math.Round(NbrAns, 0);
        NDH = (float)(NDH / NbrAns);  // nombre de jours hivernaux par ans

        var qNaCl1 = 20.83519974 * NDH + 211.3117439;   // quantité par an en g/m2 de sel déversé sur la chaussée
        var qNaCl2 = 20.83519974 * NDH - 72.9892168;  // quantité par an en g/m2 de sel déversé sur la chaussée

        // frmTempSeuil.Label3.Text = CInt(qNaCl1)
        form.concentration_annuelle_chlorure_sodium_epandage_mecanique = System.Convert.ToSingle(qNaCl1);
        // frmTempSeuil.Label74.Text = CInt(qNaCl2)
        form.concentration_chlorure = 0.001F;
        form.epaisseur_film_eau_chaussee = 2F;
        form.humidite_relative_seuil_intervention = 95F;
        form.intervalle_minimal_entre_2 = 8F;
        form.concentration_chlorure_sodium_epandage_mecanique = 36F;
        form.quantite_moyenne_chlorure_epandage_automatique = 0.5F;
        form.nb_giclage_par_intervalle = 12F;
        form.concentration_chlorure_sodium_epandage_automatique = 21F;
        form.position_de_la_1_temperature_exterieur = 1F;
        form.position_de_la_2_temperature_exterieur = 3F;
        form.attenuation_de_1_temperature_exterieur = 1F;
        form.attenuation_de_2_temperature_exterieur = 2F;
        form.difference_de_temperature_exterieur = 100F;
        form.position_de_la_1_humidite_exterieur = 2F;
        form.position_de_la_2_humidite_exterieur = 3F;
        form.attenuation_de_1_humidite_exterieur = 1F;
        form.attenuation_de_2_humidite_exterieur = 4F;
        form.difference_de_humidite_exterieur = 100F;
        form.position_de_la_1_temperature_interieure = 1F;
        form.position_de_la_2_temperature_interieure = 3F;
        form.attenuation_de_1_temperature_interieure = 1F;
        form.attenuation_de_2_temperature_interieure = 8F;
        form.difference_de_temperature_interieure = 100F;
        form.position_de_la_1_humidite_interieure = 2F;
        form.position_de_la_2_humidite_interieure = 3F;
        form.attenuation_de_1_humidite_interieure = 1F;
        form.attenuation_de_2_humidite_interieure = 1F;
        form.difference_de_humidite_interieure = 100F;
        form.concentration_annuelle_chlorure_sodium_epandage_automatique = System.Convert.ToSingle(qNaCl2);
        // frmTempSeuil.NumericUpDown1.Text = 10

        form.quantite_moyenne_chlorure_epandage_mecanique = 10;

        form.concentration_chlorure = 0.001F;
        form.epaisseur_film_eau_chaussee = 2;
        form.humidite_relative_seuil_intervention = 95;
        form.intervalle_minimal_entre_2 = 8;
        form.concentration_chlorure_sodium_epandage_mecanique = 36;
        form.quantite_moyenne_chlorure_epandage_automatique = 0.5F;
        form.nb_giclage_par_intervalle = 12;
        form.concentration_chlorure_sodium_epandage_automatique = 21;
        form.position_de_la_1_temperature_exterieur = 1;
        form.position_de_la_2_temperature_exterieur = 3;
        form.attenuation_de_1_temperature_exterieur = 1;
        form.attenuation_de_2_temperature_exterieur = 2;
        form.difference_de_temperature_exterieur = 100;
        form.position_de_la_1_humidite_exterieur = 2;
        form.position_de_la_2_humidite_exterieur = 3;
        form.attenuation_de_1_humidite_exterieur = 1;
        form.attenuation_de_2_humidite_exterieur = 4;
        form.difference_de_humidite_exterieur = 100;
        form.position_de_la_1_temperature_interieure = 1;
        form.position_de_la_2_temperature_interieure = 3;
        form.attenuation_de_1_temperature_interieure = 1;
        form.attenuation_de_2_temperature_interieure = 8;
        form.difference_de_temperature_interieure = 100;
        form.position_de_la_1_humidite_interieure = 2;
        form.position_de_la_2_humidite_interieure = 3;
        form.attenuation_de_1_humidite_interieure = 1;
        form.attenuation_de_2_humidite_interieure = 1;
        form.difference_de_humidite_interieure = 100;

        form.nb_intervention_epandage = form.quantite_moyenne_chlorure_epandage_mecanique;





        return form;
    }

    public static void InputDeicingSalt(StrctForm form)
    {


        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // Calcul du nbre d'interventions et de la quantité de sel épandu
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        bool Hiv = true;
        var Cpt = 0;
        float NDH = 0;



        // Calcul du nombre de jours hivernaux
        for (int i = 0; i <= iAnzahl - 1; i++)
        {
            if (arrDaten[i].moy6 / (double)10 > 0)
                Hiv = false;
            if (Cpt == 24)
            {
                if (Hiv == true)
                    NDH = NDH + 1;
                Hiv = true;
                Cpt = 0;
            }
            Cpt = Cpt + 1;
        }

        // frmTempSeuil = New frmMeteo
        // frmTempSeuil.Label12.Text = NbrAns

        form.nb_annees = (float)NbrAns;

        if (Math.Round(NbrAns, 1) > Math.Round(NbrAns, 0))
            NbrAns = Math.Round(NbrAns, 0) + 1;
        else
            NbrAns = Math.Round(NbrAns, 0);
        NDH = (float)(NDH / NbrAns);  // nombre de jours hivernaux par ans

        var qNaCl1 = 20.83519974 * NDH + 211.3117439;   // quantité par an en g/m2 de sel déversé sur la chaussée
        var qNaCl2 = 20.83519974 * NDH - 72.9892168;  // quantité par an en g/m2 de sel déversé sur la chaussée

        // frmTempSeuil.Label3.Text = CInt(qNaCl1)
        form.concentration_annuelle_chlorure_sodium_epandage_mecanique = System.Convert.ToSingle(qNaCl1);
        // frmTempSeuil.Label74.Text = CInt(qNaCl2)
        form.concentration_annuelle_chlorure_sodium_epandage_automatique = System.Convert.ToSingle(qNaCl2);
        // frmTempSeuil.NumericUpDown1.Text = 10

        form.quantite_moyenne_chlorure_epandage_mecanique = 10;

        // frmTempSeuil.ButtonExportFile.Hide()
        // frmTempSeuil.ButtonExportDB.Hide()
        // frmTempSeuil.LabelOR.Hide()
        // frmTempSeuil.ShowDialog()
        // frmTempSeuil.Hide()

        // calcul de la concentration en NaCl dans l'eau
        // Dim DureeInt As Short = frmTempSeuil.NumericUpDown2.Text
        int DureeInt = (int)form.intervalle_minimal_entre_2;
        // Dim QNa1 As Single = frmTempSeuil.NumericUpDown1.Text
        float QNa1 = form.quantite_moyenne_chlorure_epandage_mecanique;
        // Dim QNa2 As Single = frmTempSeuil.NumericUpDown24.Text * frmTempSeuil.NumericUpDown25.Text
        float QNa2 = form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle;
        // Dim Tseuil1 As Single = frmTempSeuil.Label22.Text
        float Tseuil1 = form.temperature_seuil_epandage_mecanique;
        // Dim Tseuil2 As Single = frmTempSeuil.Label66.Text
        float Tseuil2 = form.temperature_seuil_epandage_automatique;
        // Dim HRseuil As Single = frmTempSeuil.NumericUpDown3.Text
        float HRseuil = form.humidite_relative_seuil_intervention;
        // Dim EpNa1 As Single = frmTempSeuil.NumericUpDown4.Text / 100
        float EpNa1 = (float)(form.concentration_chlorure_sodium_epandage_mecanique / (double)100);
        // Dim EpNa2 As Single = frmTempSeuil.NumericUpDown23.Text / 100
        float EpNa2 = (float)(form.concentration_chlorure_sodium_epandage_automatique / (double)100);
        // Dim Feau As Single = frmTempSeuil.NumericUpDown5.Text
        float Feau = form.epaisseur_film_eau_chaussee;

        var Dint1 = 0;
        var Dint2 = 0;
        bool PluieOld = false;

        for (int i = 0; i <= iAnzahl - 1; i++)
        {
            if (Dint1 != 0)
                Dint1 = Dint1 + 1;
            if (Dint2 != 0)
                Dint2 = Dint2 + 1;
            if (Dint1 >= DureeInt)
                Dint1 = 0;
            if (Dint1 == 0 & arrDaten[i].moy6 / (double)10 < Tseuil1 & (arrDaten[i].moy13 / (double)10 >= HRseuil | arrDaten[i].moy17 / (double)10 > 0))
            {
                if (arrDaten[i].moy17 / (double)10 == 0)
                    arrMatrice[i].salage1 = EpNa1.ToString();
                else if (PluieOld == false)
                    arrMatrice[i].salage1 = QNa1 / (1000 * arrDaten[i].moy17 / (double)10);
                else if (i != 0)
                    arrMatrice[i].salage1 = (arrMatrice[i - 1].salage1 * 1000 * Feau + QNa1) / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                Dint1 = Dint1 + 1;
            }
            if (Dint2 == 0 & arrDaten[i].moy6 / (double)10 < Tseuil2 & (arrDaten[i].moy13 / (double)10 >= HRseuil | arrDaten[i].moy17 / (double)10 > 0))
            {
                if (arrDaten[i].moy17 / (double)10 == 0)
                    arrMatrice[i].salage2 = EpNa2;
                else if (PluieOld == false)
                    arrMatrice[i].salage2 = QNa2 / (1000 * arrDaten[i].moy17 / (double)10);
                else if (i != 0)
                    arrMatrice[i].salage2 = (arrMatrice[i - 1].salage2 * 1000 * Feau + QNa2) / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                Dint2 = Dint2 + 1;
            }
            if (arrDaten[i].moy17 / (double)10 != 0)
                PluieOld = true;
            else
                PluieOld = false;
            if (Dint1 != 1)
            {
                if (PluieOld == true)
                {
                    if (i != 0)
                        arrMatrice[i].salage1 = arrMatrice[i - 1].salage1 * 1000 * Feau / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                }
                else if (i != 0)
                    arrMatrice[i].salage1 = arrMatrice[i - 1].salage1;
                // If i = 0 Then arrMatrice(i).salage1 = frmTempSeuil.NumericUpDown6.Value / 100
                if (i == 0)
                    arrMatrice[i].salage1 = form.concentration_chlorure / (double)100;
            }
            if (Dint2 != 1)
            {
                if (PluieOld == true)
                {
                    if (i > 0)
                        arrMatrice[i].salage2 = arrMatrice[i - 1].salage2 * 1000 * Feau / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                }
                else if (i > 0)
                    arrMatrice[i].salage2 = arrMatrice[i - 1].salage2;
                // If i = 0 Then arrMatrice(i).salage2 = frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Text / 100
                if (i == 0)
                    arrMatrice[i].salage2 = form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle / (double)100;
                if (arrMatrice[i].salage2 <= 0.1 * EpNa2)
                    Dint2 = 0; // ???
            }
            if (arrMatrice[i].salage1 > EpNa1)
                arrMatrice[i].salage1 = EpNa1; // keep the maximal value
            if (arrMatrice[i].salage2 > EpNa2)
                arrMatrice[i].salage2 = EpNa2;
        }
        return form;
    }
    public static void ExportFile(StrctForm Form)
    {



        // calcul de la concentration en NaCl dans l'eau
        // Dim DureeInt As Short = frmTempSeuil.NumericUpDown2.Text
        short DureeInt = Form.intervalle_minimal_entre_2;
        // Dim QNa1 As Single = frmTempSeuil.NumericUpDown1.Text
        float QNa1 = Form.quantite_moyenne_chlorure_epandage_mecanique;
        // Dim QNa2 As Single = frmTempSeuil.NumericUpDown24.Text * frmTempSeuil.NumericUpDown25.Text
        float QNa2 = Form.quantite_moyenne_chlorure_epandage_automatique * Form.nb_giclage_par_intervalle;
        // Dim Tseuil1 As Single = frmTempSeuil.Label22.Text
        float Tseuil1 = Form.temperature_seuil_epandage_mecanique;
        // Dim Tseuil2 As Single = frmTempSeuil.Label66.Text
        float Tseuil2 = Form.temperature_seuil_epandage_automatique;
        // Dim HRseuil As Single = frmTempSeuil.NumericUpDown3.Text
        float HRseuil = Form.humidite_relative_seuil_intervention;
        // Dim EpNa1 As Single = frmTempSeuil.NumericUpDown4.Text / 100
        float EpNa1 = Form.concentration_chlorure_sodium_epandage_mecanique / (double)100;
        // Dim EpNa2 As Single = frmTempSeuil.NumericUpDown23.Text / 100
        float EpNa2 = Form.concentration_chlorure_sodium_epandage_automatique / (double)100;
        // Dim Feau As Single = frmTempSeuil.NumericUpDown5.Text
        float Feau = Form.epaisseur_film_eau_chaussee;

        short Dint1 = 0;
        short Dint2 = 0;
        bool PluieOld = false;

        for (int i = 0; i <= iAnzahl - 1; i++)
        {
            if (Dint1 != 0)
                Dint1 = Dint1 + 1;
            if (Dint2 != 0)
                Dint2 = Dint2 + 1;
            if (Dint1 >= DureeInt)
                Dint1 = 0;
            if (Dint1 == 0 & arrDaten[i].moy6 / (double)10 < Tseuil1 & (arrDaten[i].moy13 / (double)10 >= HRseuil | arrDaten[i].moy17 / (double)10 > 0))
            {
                if (arrDaten[i].moy17 / (double)10 == 0)
                    arrMatrice[i].salage1 = EpNa1;
                else if (PluieOld == false)
                    arrMatrice[i].salage1 = QNa1 / (1000 * arrDaten[i].moy17 / (double)10);
                else if (i != 0)
                    arrMatrice[i].salage1 = (arrMatrice[i - 1].salage1 * 1000 * Feau + QNa1) / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                Dint1 = Dint1 + 1;
            }
            if (Dint2 == 0 & arrDaten[i].moy6 / (double)10 < Tseuil2 & (arrDaten[i].moy13 / (double)10 >= HRseuil | arrDaten[i].moy17 / (double)10 > 0))
            {
                if (arrDaten[i].moy17 / (double)10 == 0)
                    arrMatrice[i].salage2 = EpNa2;
                else if (PluieOld == false)
                    arrMatrice[i].salage2 = QNa2 / (1000 * arrDaten[i].moy17 / (double)10);
                else if (i != 0)
                    arrMatrice[i].salage2 = (arrMatrice[i - 1].salage2 * 1000 * Feau + QNa2) / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                Dint2 = Dint2 + 1;
            }
            if (arrDaten[i].moy17 / (double)10 != 0)
                PluieOld = true;
            else
                PluieOld = false;
            if (Dint1 != 1)
            {
                if (PluieOld == true)
                {
                    if (i != 0)
                        arrMatrice[i].salage1 = arrMatrice[i - 1].salage1 * 1000 * Feau / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                }
                else if (i != 0)
                    arrMatrice[i].salage1 = arrMatrice[i - 1].salage1;
                // If i = 0 Then arrMatrice(i).salage1 = frmTempSeuil.NumericUpDown6.Value / 100
                if (i == 0)
                    arrMatrice[i].salage1 = Form.concentration_chlorure / (double)100;
            }
            if (Dint2 != 1)
            {
                if (PluieOld == true)
                {
                    if (i > 0)
                        arrMatrice[i].salage2 = arrMatrice[i - 1].salage2 * 1000 * Feau / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                }
                else if (i > 0)
                    arrMatrice[i].salage2 = arrMatrice[i - 1].salage2;
                // If i = 0 Then arrMatrice(i).salage2 = frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Text / 100
                if (i == 0)
                    arrMatrice[i].salage2 = Form.quantite_moyenne_chlorure_epandage_automatique * Form.nb_giclage_par_intervalle / (double)100;
                if (arrMatrice[i].salage2 <= 0.1 * EpNa2)
                    Dint2 = 0; // ???
            }
            if (arrMatrice[i].salage1 > EpNa1)
                arrMatrice[i].salage1 = EpNa1; // keep the maximal value
            if (arrMatrice[i].salage2 > EpNa2)
                arrMatrice[i].salage2 = EpNa2;
        }
        CalculTHS();
    }
    public static void CalculTHS()
    {
        StrctForm form;
        float[] InputMatrice = new float[DataLength + 1];
        float[] OutputMatrice = new float[DataLength + 1];

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // calcul  T et Ts
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        float a = 0.7;
        float hy = 20;

        for (int i = 0; i <= iAnzahl - 1; i++)
        {
            arrMatrice[i].T = arrDaten[i].moy6 / (double)10;
            if (arrDaten[i].moy22 < 0)
                arrDaten[i].moy22 = 0;
            arrMatrice[i].Ts = arrMatrice[i].T + (a / (double)hy) * arrDaten[i].moy22;
        }

        for (int i = 0; i <= iAnzahl - 1; i++)    // calcul de Text
            InputMatrice[i] = arrMatrice[i].T;

        // AttenBruit(CSng(frmTempSeuil.NumericUpDown8.Value), CSng(frmTempSeuil.NumericUpDown7.Value), CSng(frmTempSeuil.NumericUpDown9.Value), CSng(frmTempSeuil.NumericUpDown10.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox1.Text))
        AttenBruit(ref System.Convert.ToSingle(form.position_de_la_1_temperature_exterieur), ref System.Convert.ToSingle(form.position_de_la_2_temperature_exterieur), ref System.Convert.ToSingle(form.attenuation_de_1_temperature_exterieur), ref System.Convert.ToSingle(form.attenuation_de_2_temperature_exterieur), ref InputMatrice, ref OutputMatrice, ref System.Convert.ToSingle(form.difference_de_temperature_exterieur));
        for (int i = 0; i <= iAnzahl - 1; i++)
            arrMatrice[i].Text = OutputMatrice[i];

        arrMatrice[iAnzahl - 1].Text = InputMatrice[iAnzahl - 1];

        for (int i = 0; i <= iAnzahl - 1; i++)    // calcul de Tcaisson
            InputMatrice[i] = arrMatrice[i].T;

        // AttenBruit(CSng(frmTempSeuil.NumericUpDown21.Value), CSng(frmTempSeuil.NumericUpDown22.Value), CSng(frmTempSeuil.NumericUpDown19.Value), CSng(frmTempSeuil.NumericUpDown20.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox4.Text))
        AttenBruit(ref System.Convert.ToSingle(form.position_de_la_1_temperature_interieure), ref System.Convert.ToSingle(form.position_de_la_2_temperature_interieure), ref System.Convert.ToSingle(form.attenuation_de_1_temperature_interieure), ref System.Convert.ToSingle(form.attenuation_de_2_temperature_interieure), ref InputMatrice, ref OutputMatrice, ref System.Convert.ToSingle(form.difference_de_temperature_interieure));

        for (int i = 0; i <= iAnzahl - 1; i++)
            arrMatrice[i].Tcaisson = OutputMatrice[i];
        arrMatrice[iAnzahl - 1].Tcaisson = InputMatrice[iAnzahl - 1];

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // calculs d'exposition HR
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        /// 
        short NbPluie = 0; // Ajout Bitume TSANCHEZ
        short NbPluieMax = 50; // valeu par défaut

        for (int i = 0; i <= (iAnzahl - 1); i++)
        {
            if (arrDaten[i].moy13 >= 1000)
            {
                arrMatrice[i].HR_brouillard = 99.99;
                arrMatrice[i].HR_bitume = arrMatrice[i].HR_brouillard; // Ajout Bitume TSANCHEZ
            }
            else
            {
                arrMatrice[i].HR_brouillard = arrDaten[i].moy13 / (double)10;
                arrMatrice[i].HR_bitume = arrMatrice[i].HR_brouillard; // Ajout Bitume TSANCHEZ
            }

            if (i > 0)
            {
                if (arrDaten[i].moy17 != 0 & arrDaten[i - 1].moy17 != 0)
                    arrMatrice[i].HR_eclaboussures = 100;
                else if (arrDaten[i].moy13 >= 1000)
                    arrMatrice[i].HR_eclaboussures = 99.99;
                else
                    arrMatrice[i].HR_eclaboussures = arrDaten[i].moy13 / (double)10;
            }

            if (arrMatrice[i].hour > 17 | arrMatrice[i].hour < 6)
            {
                // pendant la nuit (de 18h00 à 6h00)
                if (arrDaten[i].moy17 != 0)
                {
                    arrMatrice[i].HR_direct = 100;
                    NbPluie += 1;
                    if (NbPluie == NbPluieMax)
                    {
                        arrMatrice[i].HR_bitume = 100;
                        NbPluie = 0;
                    }
                }
                else if (arrDaten[i].moy13 >= 1000)
                    arrMatrice[i].HR_direct = 99.99;
                else
                    arrMatrice[i].HR_direct = arrDaten[i].moy13 / (double)10;
            }
            else if (arrDaten[i].moy17 != 0)
                arrMatrice[i].HR_direct = 100;
            else if (arrDaten[i].moy13 >= 1000)
                arrMatrice[i].HR_direct = 99.99;
            else
                arrMatrice[i].HR_direct = arrDaten[i].moy13 / (double)10;
        }

        for (int i = 0; i <= iAnzahl - 1; i++)    // calcul de HRext
            InputMatrice[i] = arrMatrice[i].HR_brouillard;
        // AttenBruit(CSng(frmTempSeuil.NumericUpDown13.Value), CSng(frmTempSeuil.NumericUpDown14.Value), CSng(frmTempSeuil.NumericUpDown11.Value), CSng(frmTempSeuil.NumericUpDown12.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox2.Text))
        AttenBruit(ref System.Convert.ToSingle(form.position_de_la_1_humidite_exterieur), ref System.Convert.ToSingle(form.position_de_la_2_humidite_exterieur), ref System.Convert.ToSingle(form.attenuation_de_1_humidite_exterieur), ref System.Convert.ToSingle(form.attenuation_de_2_humidite_exterieur), ref InputMatrice, ref OutputMatrice, ref System.Convert.ToSingle(form.difference_de_humidite_exterieur));

        for (int i = 0; i <= iAnzahl - 1; i++)
            arrMatrice[i].HR_ext = OutputMatrice[i];
        arrMatrice[iAnzahl - 1].HR_ext = InputMatrice[iAnzahl - 1];

        for (int i = 0; i <= iAnzahl - 1; i++)    // calcul de HRcaisson
            InputMatrice[i] = arrMatrice[i].HR_brouillard;
        // AttenBruit(CSng(frmTempSeuil.NumericUpDown17.Value), CSng(frmTempSeuil.NumericUpDown18.Value), CSng(frmTempSeuil.NumericUpDown15.Value), CSng(frmTempSeuil.NumericUpDown16.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox3.Text))
        AttenBruit(ref System.Convert.ToSingle(form.position_de_la_1_humidite_interieure), ref System.Convert.ToSingle(form.position_de_la_2_humidite_interieure), ref System.Convert.ToSingle(form.attenuation_de_1_humidite_interieure), ref System.Convert.ToSingle(form.attenuation_de_2_humidite_interieure), ref InputMatrice, ref OutputMatrice, ref System.Convert.ToSingle(form.difference_de_humidite_interieure));

        for (int i = 0; i <= iAnzahl - 1; i++)
            arrMatrice[i].HR_caisson = OutputMatrice[i];
        arrMatrice[iAnzahl - 1].HR_caisson = InputMatrice[iAnzahl - 1];
    }

    public static void WriteExpoFile(ref string OutFile, ref string PostFile, ref string txtFile, ref bool Canc)
    {

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // création des fichiers INPUT
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        System.IO.TextWriter INFile1, INFile2, INFile3, INFile4, INFile5, INFile6, INFile7, INFile8, INFile9, INFile10, INFile11, INFile12, INFile13, INFile14, INFile15, INFile16, INFile17, INFile18;
        PostFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../public/exports/");
        OutFile = PostFile + "EXPO_M_E_E_" + txtFile + ".txt";
        INFile1 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_E_O_" + txtFile + ".txt";
        INFile2 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_B_E_" + txtFile + ".txt";
        INFile3 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_B_O_" + txtFile + ".txt";
        INFile4 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_D_E_" + txtFile + ".txt";
        INFile5 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_D_O_" + txtFile + ".txt";
        INFile6 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_EXT_" + txtFile + ".txt";
        INFile7 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_CAI_" + txtFile + ".txt";
        INFile8 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_CAC_" + txtFile + ".txt";
        INFile9 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_E_E_" + txtFile + ".txt";
        INFile10 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_E_O_" + txtFile + ".txt";
        INFile11 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_B_E_" + txtFile + ".txt";
        INFile12 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_B_O_" + txtFile + ".txt";
        INFile13 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_D_E_" + txtFile + ".txt";
        INFile14 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_D_O_" + txtFile + ".txt";
        INFile15 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_EXT_" + txtFile + ".txt";
        INFile16 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_A_CAC_" + txtFile + ".txt";
        INFile17 = System.IO.File.CreateText(OutFile);
        OutFile = PostFile + "EXPO_M_BIT_" + txtFile + ".txt";
        INFile18 = System.IO.File.CreateText(OutFile);

        Meteo_File[] arrINPUT_M_E_E = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_E_O = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_B_E = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_B_O = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_D_E = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_D_O = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_EXT = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_CAI = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_CAC = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_E_E = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_E_O = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_B_E = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_B_O = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_D_E = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_D_O = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_EXT = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_A_CAC = new Meteo_File[iAnzahl - 1 + 1];
        Meteo_File[] arrINPUT_M_BIT = new Meteo_File[iAnzahl - 1 + 1];

        for (int i = 0; i <= iAnzahl - 1; i++)
        {
            // eclaboussure et ensoleillement
            arrINPUT_M_E_E[i].HR = arrMatrice[i].HR_eclaboussures;
            arrINPUT_M_E_E[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_E_E[i].Tsurf = arrMatrice[i].Ts;
            // eclaboussure et ombrée
            arrINPUT_M_E_O[i].HR = arrMatrice[i].HR_eclaboussures;
            arrINPUT_M_E_O[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_E_O[i].Tsurf = arrMatrice[i].T;
            // brouillard et ensoleillement
            arrINPUT_M_B_E[i].HR = arrMatrice[i].HR_brouillard;
            arrINPUT_M_B_E[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_B_E[i].Tsurf = arrMatrice[i].Ts;
            // brouillard et ombrée
            arrINPUT_M_B_O[i].HR = arrMatrice[i].HR_brouillard;
            arrINPUT_M_B_O[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_B_O[i].Tsurf = arrMatrice[i].T;
            // direct et ensoleillement
            arrINPUT_M_D_E[i].HR = arrMatrice[i].HR_direct;
            arrINPUT_M_D_E[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_D_E[i].Tsurf = arrMatrice[i].Ts;
            // direct et ombrée
            arrINPUT_M_D_O[i].HR = arrMatrice[i].HR_direct;
            arrINPUT_M_D_O[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_D_O[i].Tsurf = arrMatrice[i].T;
            // extérieur et à l'abris des intempéries
            arrINPUT_M_EXT[i].HR = arrMatrice[i].HR_ext;
            arrINPUT_M_EXT[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_EXT[i].Tsurf = arrMatrice[i].Text;
            // intérieur du caisson et sans sel
            arrINPUT_M_CAI[i].HR = arrMatrice[i].HR_caisson;
            arrINPUT_M_CAI[i].Sel = 0;
            arrINPUT_M_CAI[i].Tsurf = arrMatrice[i].Tcaisson;
            // intérieur du caisson et avec présence de sel
            arrINPUT_M_CAC[i].HR = arrMatrice[i].HR_caisson;
            arrINPUT_M_CAC[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_CAC[i].Tsurf = arrMatrice[i].Tcaisson;
            // Statgnant avec Bitume TSANCHEZ
            arrINPUT_M_BIT[i].HR = arrMatrice[i].HR_bitume;
            arrINPUT_M_BIT[i].Sel = arrMatrice[i].salage1;
            arrINPUT_M_BIT[i].Tsurf = arrMatrice[i].T;

            // eclaboussure et ensoleillement
            arrINPUT_A_E_E[i].HR = arrMatrice[i].HR_eclaboussures;
            arrINPUT_A_E_E[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_E_E[i].Tsurf = arrMatrice[i].Ts;
            // eclaboussure et ombrée
            arrINPUT_A_E_O[i].HR = arrMatrice[i].HR_eclaboussures;
            arrINPUT_A_E_O[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_E_O[i].Tsurf = arrMatrice[i].T;
            // brouillard et ensoleillement
            arrINPUT_A_B_E[i].HR = arrMatrice[i].HR_brouillard;
            arrINPUT_A_B_E[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_B_E[i].Tsurf = arrMatrice[i].Ts;
            // brouillard et ombrée
            arrINPUT_A_B_O[i].HR = arrMatrice[i].HR_brouillard;
            arrINPUT_A_B_O[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_B_O[i].Tsurf = arrMatrice[i].T;
            // direct et ensoleillement
            arrINPUT_A_D_E[i].HR = arrMatrice[i].HR_direct;
            arrINPUT_A_D_E[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_D_E[i].Tsurf = arrMatrice[i].Ts;
            // direct et ombrée
            arrINPUT_A_D_O[i].HR = arrMatrice[i].HR_direct;
            arrINPUT_A_D_O[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_D_O[i].Tsurf = arrMatrice[i].T;
            // extérieur et à l'abris des intempéries
            arrINPUT_A_EXT[i].HR = arrMatrice[i].HR_ext;
            arrINPUT_A_EXT[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_EXT[i].Tsurf = arrMatrice[i].Text;
            // intérieur du caisson et avec présence de sel
            arrINPUT_A_CAC[i].HR = arrMatrice[i].HR_caisson;
            arrINPUT_A_CAC[i].Sel = arrMatrice[i].salage2;
            arrINPUT_A_CAC[i].Tsurf = arrMatrice[i].Tcaisson;
        }

        // écriture dans les fichiers
        INFile1.WriteLine(iAnzahl);
        INFile1.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile1.WriteLine(arrINPUT_M_E_E[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_E_E[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_E_E[i].Tsurf, i);
        INFile1.Close();

        INFile2.WriteLine(iAnzahl);
        INFile2.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile2.WriteLine(arrINPUT_M_E_O[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_E_O[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_E_O[i].Tsurf, i);
        INFile2.Close();

        INFile3.WriteLine(iAnzahl);
        INFile3.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile3.WriteLine(arrINPUT_M_B_E[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_B_E[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_B_E[i].Tsurf, i);
        INFile3.Close();

        INFile4.WriteLine(iAnzahl);
        INFile4.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile4.WriteLine(arrINPUT_M_B_O[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_B_O[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_B_O[i].Tsurf, i);
        INFile4.Close();

        INFile5.WriteLine(iAnzahl);
        INFile5.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile5.WriteLine(arrINPUT_M_D_E[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_D_E[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_D_E[i].Tsurf, i);
        INFile5.Close();

        INFile6.WriteLine(iAnzahl);
        INFile6.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile6.WriteLine(arrINPUT_M_D_O[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_D_O[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_D_O[i].Tsurf, i);
        INFile6.Close();

        INFile7.WriteLine(iAnzahl);
        INFile7.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile7.WriteLine(arrINPUT_M_EXT[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_EXT[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_EXT[i].Tsurf, i);
        INFile7.Close();

        INFile8.WriteLine(iAnzahl);
        INFile8.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile8.WriteLine(arrINPUT_M_CAI[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_CAI[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_CAI[i].Tsurf, i);
        INFile8.Close();

        INFile9.WriteLine(iAnzahl);
        INFile9.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile9.WriteLine(arrINPUT_M_CAC[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_CAC[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_CAC[i].Tsurf, i);
        INFile9.Close();

        INFile10.WriteLine(iAnzahl);
        INFile10.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile10.WriteLine(arrINPUT_A_E_E[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_E_E[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_E_E[i].Tsurf, i);
        INFile10.Close();

        INFile11.WriteLine(iAnzahl);
        INFile11.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile11.WriteLine(arrINPUT_A_E_O[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_E_O[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_E_O[i].Tsurf, i);
        INFile11.Close();

        INFile12.WriteLine(iAnzahl);
        INFile12.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile12.WriteLine(arrINPUT_A_B_E[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_B_E[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_B_E[i].Tsurf, i);
        INFile12.Close();

        INFile13.WriteLine(iAnzahl);
        INFile13.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile13.WriteLine(arrINPUT_A_B_O[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_B_O[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_B_O[i].Tsurf, i);
        INFile13.Close();

        INFile14.WriteLine(iAnzahl);
        INFile14.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile14.WriteLine(arrINPUT_A_D_E[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_D_E[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_D_E[i].Tsurf, i);
        INFile14.Close();

        INFile15.WriteLine(iAnzahl);
        INFile15.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile15.WriteLine(arrINPUT_A_D_O[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_D_O[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_D_O[i].Tsurf, i);
        INFile15.Close();

        INFile16.WriteLine(iAnzahl);
        INFile16.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile16.WriteLine(arrINPUT_A_EXT[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_EXT[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_EXT[i].Tsurf, i);
        INFile16.Close();

        INFile17.WriteLine(iAnzahl);
        INFile17.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile17.WriteLine(arrINPUT_A_CAC[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_A_CAC[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_A_CAC[i].Tsurf, i);
        INFile17.Close();

        INFile18.WriteLine(iAnzahl);
        INFile18.WriteLine("3600");
        for (int i = 0; i <= iAnzahl - 1; i++)
            INFile18.WriteLine(arrINPUT_M_BIT[i].HR + Constants.vbTab + Constants.vbTab + arrINPUT_M_BIT[i].Sel + Constants.vbTab + Constants.vbTab + arrINPUT_M_BIT[i].Tsurf, i);
        INFile18.Close();
    }

    public static string MeteoTreatmentTroubleshootingPart1(string outfile)
    {
        // Dim outfile As String
        var PostFile = "";
        var txtfile = "";
        bool Canc = false;

        ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);
        string Retour = Troubleshoot(0);
        return Retour;
    }
    public static string MeteoTreatmentTroubleshootingPart2(string outfile)
    {
        // Dim outfile As String
        var PostFile = "";
        var txtfile = "";
        bool Canc = false;

        ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);
        string Retour = Troubleshoot(1);
        return Retour;
    }

    public static void MeteoTreatmentInputDeicingSalt(string outfile)
    {
        // Dim outfile As String
        var PostFile = "";
        var txtfile = "";
        bool Canc = false;

        ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);
        StrctForm Form;
        InputDeicingSalt(Form);
    }

    public static void MeteoTreatmentPrecalcul(string outfile)
    {
        StrctForm Form;
        Form = precalcul(outfile, Form);
        string Fichier = @"C:\Users\Public\Documents\TempSeuil.txt";
        WriteMeteoFormToTextFile(Fichier, Form, false);
        return Fichier;
    }
    public static void MeteoTreatment()
    {
        string outfile;
        string PostFile;
        string txtfile;
        bool Canc = false;

        // ReadMeteoFile(outfile, PostFile, txtfile, Canc)
        // If Canc = True Then End

        // Troubleshoot()

        // InputDeicingSalt()

        CalculTHS();

        WriteExpoFile(ref outfile, ref PostFile, ref txtfile, ref Canc);
    }
    public static void MeteoTreatmentCalcul(List<string> outfiles)
    {
        string PostFile;
        string txtfile;
        bool Canc = false;
        ReadMeteoFile(outfiles[0], ref PostFile, ref txtfile, ref Canc);
        Troubleshoot(-1);
        StrctForm Form;
        Form = InputDeicingSalt(Form);
        Form = precalcul(outfiles[0], Form);
        Form = WCal(outfiles[1], Form);
        string Fichier = @"C:\Users\Public\Documents\TempSeuil.txt";
        WriteMeteoFormToTextFile(Fichier, Form, true);
        return Fichier;
    }

    public static void MeteoTreatmentExport(List<string> outfiles)
    {
        string PostFile;
        string txtfile;
        bool Canc = false;
        StrctForm form = new StrctForm();
        ReadMeteoFile(outfiles[0], ref PostFile, ref txtfile, ref Canc);
        Troubleshoot(-1);
        form = ReadMeteoFormFromTextFile(outfiles[1]);
        ExportFile(form);
        WriteExpoFile(ref outfiles[1], ref PostFile, ref txtfile, ref Canc);

        return "good";
    }
    public static void WCal(string file, StrctForm form)
    {
        double NbrAns; // [-]
        int i; // [-]
        int DureeIntrvent; // [h]
        long nbrInt1 = 0;
        long nbrInt2 = 0;
        float Tseuil1 = -9;  // °C
        float Tseuil2 = -9;  // °C
        float HRseuil = 0;
        long Nint1 = 0;
        long Nint2 = 0;
        short Dint = 0;
        bool PluieOld = false;
        float QNa = 0;
        float EpNa = 0;
        float Feau;
        string Na;

        CalNeige();

        // DureeIntrvent = frmTempSeuil.NumericUpDown2.Value
        DureeIntrvent = form.intervalle_minimal_entre_2;
        // nbrInt1 = CInt(frmTempSeuil.Label3.Text / frmTempSeuil.NumericUpDown1.Value)
        nbrInt1 = System.Convert.ToInt64(form.concentration_annuelle_chlorure_sodium_epandage_mecanique / (double)form.quantite_moyenne_chlorure_epandage_mecanique);
        // nbrInt2 = CInt(frmTempSeuil.Label74.Text / (frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Value))
        nbrInt2 = System.Convert.ToInt64(form.concentration_annuelle_chlorure_sodium_epandage_automatique / (double)(form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle));
        // frmTempSeuil.Label6.Text = nbrInt1
        form.nb_intervention_epandage = nbrInt1;
        // frmTempSeuil.Label76.Text = nbrInt2
        form.nb_giclages_annuel = nbrInt2;
        // NbrAns = frmTempSeuil.Label12.Text
        NbrAns = form.nb_annees;
        nbrInt1 = System.Convert.ToInt64(nbrInt1 * NbrAns);
        nbrInt2 = System.Convert.ToInt64(nbrInt2 * NbrAns);
        // HRseuil = frmTempSeuil.NumericUpDown3.Value
        HRseuil = form.humidite_relative_seuil_intervention;
        while (Nint1 < nbrInt1)
        {
            Nint1 = 0;
            Dint = 0;
            for (i = 0; i <= iAnzahl - 1; i++)
            {
                if (Dint != 0)
                    Dint = Dint + 1;
                if (Dint >= DureeIntrvent)
                    Dint = 0;
                if (Dint == 0 & arrDaten[i].moy6 / (double)10 < Tseuil1 & (arrDaten[i].moy13 / (double)10 >= HRseuil | arrDaten[i].moy17 / (double)10 > 0))
                {
                    Nint1 += 1;
                    Dint += 1;
                }
            }
            Tseuil1 += 0.1;
        }
        // frmTempSeuil.Label22.Text = CInt(Tseuil1 * 10) / 10
        form.temperature_seuil_epandage_mecanique = System.Convert.ToDouble(Tseuil1 * 10) / (double)10;
        while (Nint2 < nbrInt2)
        {
            Nint2 = 0;
            Dint = 0;
            Na = 0;
            // EpNa = frmTempSeuil.NumericUpDown23.Text / 100
            EpNa = form.concentration_chlorure_sodium_epandage_automatique / (double)100;
            // QNa = frmTempSeuil.NumericUpDown24.Text * frmTempSeuil.NumericUpDown25.Text
            QNa = form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle;
            PluieOld = false;
            // Feau = frmTempSeuil.NumericUpDown5.Text
            Feau = form.epaisseur_film_eau_chaussee;
            for (i = 0; i <= iAnzahl - 1; i++)
            {
                if (Dint != 0)
                    Dint = Dint + 1;
                if (Dint == 0 & arrDaten[i].moy6 / (double)10 < Tseuil2 & (arrDaten[i].moy13 / (double)10 >= HRseuil | arrDaten[i].moy17 / (double)10 > 0))
                {
                    if (arrDaten[i].moy17 / (double)10 == 0)
                        Na = EpNa;
                    else if (PluieOld == false)
                        Na = QNa / (1000 * arrDaten[i].moy17 / (double)10);
                    else if (i != 0)
                        Na = (Na * 1000 * Feau + QNa) / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                    Dint = Dint + 1;
                    Nint2 = Nint2 + 1;
                }
                if (arrDaten[i].moy17 / (double)10 != 0)
                    PluieOld = true;
                else
                    PluieOld = false;
                if (Dint != 1)
                {
                    if (PluieOld == true)
                    {
                        if (i > 0)
                            Na = Na * 1000 * Feau / ((Feau + arrDaten[i].moy17 / (double)10) * 1000);
                    }
                    // If i = 0 Then Na = frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Text / 100
                    if (i == 0)
                        Na = form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle / (double)100;
                    if (Na <= 0.1 * EpNa)
                        Dint = 0;
                }
                if (Na > EpNa)
                    Na = EpNa;
            }
            Tseuil2 = Tseuil2 + 0.1;
        }
        // frmTempSeuil.Label24.Text = CInt(Tseuil2 * 10) / 10
        form.temperature_seuil_epandage_automatique = System.Convert.ToDouble(Tseuil2 * 10) / (double)10;
        return form;
    }

    private static void CalNeige()
    {
        int i; // [-]
        float SeuilNeige; // °C

        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // répartition de la neige
        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        int k;
        float roh; // densité de neige [kg/m3]
        float cum = 0;
        SeuilNeige = 4;
        if (CasInput == 1)
        {
            for (i = 0; i <= iAnzahl - 1; i++)
            {
                if (arrDaten[i].moy80 == 32767 | arrDaten[i].moy80 == 0)
                {
                    k = k + 1;
                    if (k > 15)
                        k = 15;
                }
                if (arrDaten[i].moy80 != 0 & arrDaten[i].moy80 != 32767)
                {
                    for (k = 0; k <= k - 1; k++)
                    {
                        if (arrDaten[i - k].moy6 / (double)10 >= SeuilNeige)
                            arrDaten[i - k].neige = 0;
                        else
                        {
                            if ((arrDaten[i - k].moy6 / (double)10) <= -1)
                                roh = 3 * arrDaten[i - k].moy6 / (double)10 + 110;
                            else
                                roh = 23 * arrDaten[i - k].moy6 / (double)10 + 130;
                            arrDaten[i - k].neige = (arrDaten[i - k].moy17 / (double)10) * 1000 / roh;  // 1000 densité de l'eau=cte
                        }
                    }
                }
            }
        }
        else
            for (i = 0; i <= iAnzahl - 1; i++)
            {
                if (arrDaten[i].moy6 / (double)10 >= SeuilNeige)
                    arrDaten[i].neige = 0;
                else
                {
                    if ((arrDaten[i].moy6 / (double)10) <= -1)
                        roh = 3 * arrDaten[i].moy6 / (double)10 + 110;
                    else
                        roh = 23 * arrDaten[i].moy6 / (double)10 + 130;
                    arrDaten[i].neige = (arrDaten[i].moy17 / (double)10) * 1000 / roh;    // 1000 densité de l'eau=cte
                }
                cum = cum + arrDaten[i].neige;
                if (cum < 2)
                    arrDaten[i].neige = 0;
                if (arrDaten[i].moy80 == 0)
                    cum = 0;
            }
    }

    private static void AttenBruit(ref float A, ref float B, ref float C, ref float D, ref float[] tempInput, ref float[] tempOutput, ref float tlim)
    {
        float dT1;
        float dT2;
        float bT1;
        float bT2;
        float T1;
        float T2;
        int i;
        int j;
        int k;
        int l;
        bool PentePos;

        for (i = 0; i <= iAnzahl - 1; i++)
        {
            k = l;
            for (j = k; j <= iAnzahl - 3; j++) // trouve le min et le max température 
            {
                dT1 = tempInput[j + 1] - tempInput[j];
                dT2 = tempInput[j + 2] - tempInput[j + 1];
                if (j == k)
                    bT1 = tempInput[j];
                if (dT1 > 0 & j == k)
                    PentePos = true;
                else if (dT1 < 0 & j == k)
                    PentePos = false;
                else if (dT1 == 0 & j == k)
                    bT1 = tempInput[j + 1];
                if (PentePos == true & dT2 < 0)
                {
                    bT2 = tempInput[j + 1];
                    break;
                }
                else if (PentePos == false & dT2 > 0)
                {
                    bT2 = tempInput[j + 1];
                    break;
                }
            }
            bT1 = A * (bT2 - bT1) / (double)B + bT1;  // calcul de la moyenne
            for (l = k; l <= j; l++)
            {
                dT1 = tempInput[l + 1] - tempInput[l];
                if (dT1 != 0)
                    dT2 = C / (double)(D * dT1);
                else
                    dT2 = 0;
                if (System.Math.Abs(dT2) > tlim)
                    dT2 = 0;
                dT1 = dT1 * dT2;
                if (l == 0)
                {
                    T1 = tempInput[l] - dT1;
                    T2 = tempInput[l] + dT1;
                }
                else
                {
                    T1 = tempOutput[l - 1] - dT1;
                    T2 = tempOutput[l - 1] + dT1;
                }
                if (System.Math.Abs(bT1 - T2) < System.Math.Abs(bT1 - T1))
                    tempOutput[l] = T2;
                else
                    tempOutput[l] = T1;
            }
            if (l == iAnzahl - 1)
                break;
        }
    }


    public static void WriteMeteoFormToTextFile(string outfile, StrctForm form, bool isCalcul)
    {
        // Ouvrir le fichier texte en mode écriture
        int nFic = FileSystem.FreeFile();
        System.IO.StreamWriter sw = new System.IO.StreamWriter(outfile);


        sw.WriteLine(form.nb_annees);
        sw.WriteLine(form.concentration_chlorure);
        sw.WriteLine(form.epaisseur_film_eau_chaussee);
        sw.WriteLine(form.humidite_relative_seuil_intervention);

        sw.WriteLine(form.concentration_annuelle_chlorure_sodium_epandage_mecanique);
        sw.WriteLine(form.quantite_moyenne_chlorure_epandage_mecanique);
        if (isCalcul)
            sw.WriteLine(form.nb_intervention_epandage);
        // PrintLine(nFic, frmTempSeuil.Label6.Text)
        sw.WriteLine(form.intervalle_minimal_entre_2);
        sw.WriteLine(form.concentration_chlorure_sodium_epandage_mecanique);
        // PrintLine(nFic, frmTempSeuil.Label22.Text)
        if (isCalcul)
            sw.WriteLine(form.temperature_seuil_epandage_mecanique);
        sw.WriteLine(form.concentration_annuelle_chlorure_sodium_epandage_automatique);
        sw.WriteLine(form.quantite_moyenne_chlorure_epandage_automatique);
        if (isCalcul)
            sw.WriteLine(form.nb_giclages_annuel);
        // PrintLine(nFic, frmTempSeuil.Label76.Text)
        sw.WriteLine(form.nb_giclage_par_intervalle);
        sw.WriteLine(form.concentration_chlorure_sodium_epandage_automatique);
        // PrintLine(nFic, frmTempSeuil.Label66.Text)
        if (isCalcul)
            sw.WriteLine(form.temperature_seuil_epandage_automatique);
        sw.WriteLine(form.position_de_la_1_temperature_exterieur);
        sw.WriteLine(form.position_de_la_2_temperature_exterieur);
        sw.WriteLine(form.attenuation_de_1_temperature_exterieur);
        sw.WriteLine(form.attenuation_de_2_temperature_exterieur);
        sw.WriteLine(form.difference_de_temperature_exterieur);
        sw.WriteLine(form.position_de_la_1_humidite_exterieur);
        sw.WriteLine(form.position_de_la_2_humidite_exterieur);
        sw.WriteLine(form.attenuation_de_1_humidite_exterieur);
        sw.WriteLine(form.attenuation_de_2_humidite_exterieur);
        sw.WriteLine(form.difference_de_humidite_exterieur);

        sw.WriteLine(form.position_de_la_1_temperature_interieure);
        sw.WriteLine(form.position_de_la_2_temperature_interieure);
        sw.WriteLine(form.attenuation_de_1_temperature_interieure);
        sw.WriteLine(form.attenuation_de_2_temperature_interieure);
        sw.WriteLine(form.difference_de_temperature_interieure);
        sw.WriteLine(form.position_de_la_1_humidite_interieure);
        sw.WriteLine(form.position_de_la_2_humidite_interieure);
        sw.WriteLine(form.attenuation_de_1_humidite_interieure);
        sw.WriteLine(form.attenuation_de_2_humidite_interieure);
        sw.WriteLine(form.difference_de_humidite_interieure);
        sw.Close();
    }

    public static StrctForm ReadMeteoFormFromTextFile(string infile)
    {
        StrctForm form = new StrctForm();

        // Vérifier si le fichier existe
        if (!File.Exists(infile))
            throw new FileNotFoundException("Le fichier spécifié n'existe pas.", infile);

        // Lire le fichier ligne par ligne
        string[] lines = File.ReadAllLines(infile);
        int index = 0;

        try
        {
            form.nb_annees = double.Parse(lines[index]); index += 1;
            form.concentration_chlorure = double.Parse(lines[index]); index += 1;
            form.epaisseur_film_eau_chaussee = double.Parse(lines[index]); index += 1;
            form.humidite_relative_seuil_intervention = double.Parse(lines[index]); index += 1;

            form.concentration_annuelle_chlorure_sodium_epandage_mecanique = double.Parse(lines[index]); index += 1;
            form.quantite_moyenne_chlorure_epandage_mecanique = double.Parse(lines[index]); index += 1;

            form.nb_intervention_epandage = double.Parse(lines[index]); index += 1;

            form.intervalle_minimal_entre_2 = double.Parse(lines[index]); index += 1;
            form.concentration_chlorure_sodium_epandage_mecanique = double.Parse(lines[index]); index += 1;

            form.temperature_seuil_epandage_mecanique = double.Parse(lines[index]); index += 1;

            form.concentration_annuelle_chlorure_sodium_epandage_automatique = double.Parse(lines[index]); index += 1;
            form.quantite_moyenne_chlorure_epandage_automatique = double.Parse(lines[index]); index += 1;

            form.nb_giclages_annuel = double.Parse(lines[index]); index += 1;

            form.nb_giclage_par_intervalle = double.Parse(lines[index]); index += 1;
            form.concentration_chlorure_sodium_epandage_automatique = double.Parse(lines[index]); index += 1;

            form.temperature_seuil_epandage_automatique = double.Parse(lines[index]); index += 1;

            form.position_de_la_1_temperature_exterieur = double.Parse(lines[index]); index += 1;
            form.position_de_la_2_temperature_exterieur = double.Parse(lines[index]); index += 1;
            form.attenuation_de_1_temperature_exterieur = double.Parse(lines[index]); index += 1;
            form.attenuation_de_2_temperature_exterieur = double.Parse(lines[index]); index += 1;
            form.difference_de_temperature_exterieur = double.Parse(lines[index]); index += 1;

            form.position_de_la_1_humidite_exterieur = double.Parse(lines[index]); index += 1;
            form.position_de_la_2_humidite_exterieur = double.Parse(lines[index]); index += 1;
            form.attenuation_de_1_humidite_exterieur = double.Parse(lines[index]); index += 1;
            form.attenuation_de_2_humidite_exterieur = double.Parse(lines[index]); index += 1;
            form.difference_de_humidite_exterieur = double.Parse(lines[index]); index += 1;

            form.position_de_la_1_temperature_interieure = double.Parse(lines[index]); index += 1;
            form.position_de_la_2_temperature_interieure = double.Parse(lines[index]); index += 1;
            form.attenuation_de_1_temperature_interieure = double.Parse(lines[index]); index += 1;
            form.attenuation_de_2_temperature_interieure = double.Parse(lines[index]); index += 1;
            form.difference_de_temperature_interieure = double.Parse(lines[index]); index += 1;

            form.position_de_la_1_humidite_interieure = double.Parse(lines[index]); index += 1;
            form.position_de_la_2_humidite_interieure = double.Parse(lines[index]); index += 1;
            form.attenuation_de_1_humidite_interieure = double.Parse(lines[index]); index += 1;
            form.attenuation_de_2_humidite_interieure = double.Parse(lines[index]); index += 1;
            form.difference_de_humidite_interieure = double.Parse(lines[index]); index += 1;
        }
        catch (Exception ex)
        {
            throw new FormatException("Erreur lors de la lecture du fichier. Assurez-vous qu'il suit le bon format.", ex);
        }

        return form;
    }
}
