using System;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace TransChlorApi
{

    class Meteo
    {

        private static int DataLength = 440000;
        private StrctPanne[] arrPanne = new StrctPanne[DataLength + 1]; // matrice d'analyse des pannes, conçu pour 50ans mesure chaque heure
        private StrctCalc[] arrMatrice = new StrctCalc[DataLength + 1]; // matrice de calcul, conçu pour 50ans mesure chaque heure
        private StrctMeteo[] arrDaten = new StrctMeteo[DataLength + 1]; // matrice input météo, conçu pour 50ans mesure chaque heure
                                                                               // Dim frmTempSeuil As frmMeteo
        private int iAnzahl; // nombre de ligne
        private double NbrAns;
        private string Export;
        private short CasInput;

        public struct StrctMeteo // colonnes de la matrice à partir du fichier METEO_*.txt
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

        public struct StrctCalc // colonnes de la matrice de calcul
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

        public struct StrctPanne // colonnes de la matrice des pannes
        {
            public int PanneStart; // colonnes début des pannes
            public int PanneEnd; // colonnes fin des pannes
            public string PanneMesure; // colonnes des types de pannes
        }

        public struct Meteo_File // fichier INPUT
        {
            public float HR; // colonnes HR
            public float Sel; // colonnes salage
            public float Tsurf; // colonnes Température de surface (T ou Ts)

            internal static void WriteAllBytes(string savePath, byte[] fileData)
            {
                throw new NotImplementedException();
            }
        }

        public void SetExport(ref string Value)
        {

            Export = Value;

        }

        public void FilePost(ref string outfile, ref string PostFile)
        {
            PostFile = Path.GetDirectoryName(outfile);
            if (!string.IsNullOrEmpty(PostFile))
            {
                FileSystem.ChDir(PostFile);
            }
            else
            {
                Console.WriteLine("⚠️ Impossible de déterminer le répertoire de : " + outfile);
            }
        }


        public void FileOnly(ref string outfile)
        {
            short iPos;
            string Dim1;

            iPos = 10;
            Dim1 = @"\";
            while (iPos > 0)
            {
                iPos = (short)Strings.InStr(1, outfile, Dim1, CompareMethod.Text);
                if (iPos != 0)
                {
                    iPos = (short)(Strings.Len(outfile) - iPos);
                    outfile = Strings.Right(outfile, iPos);
                }
            }

        }


        public void ReadMeteoFile(string OutFile, ref string PostFile, ref string txtFile, ref bool Canc)
        {

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // lecture fichier METEO_*.txt
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            string Filtre = "Text files (METEO_*.txt)|METEO_*.txt";
            short Index = 1;
            bool Directoire = true;
            string Titre = "Sélectionner le fichier météo";

            // OpenDialog(OutFile, Canc, Filtre, Index, Directoire, Titre)
            // If Canc = True Then End

            int nFic = FileSystem.FreeFile();

            FileSystem.FileOpen(nFic, OutFile, OpenMode.Input, OpenAccess.Read, OpenShare.Shared);
            FilePost(ref OutFile, ref PostFile);
            FileOnly(ref OutFile);
            // int posTxt;
            // posTxt = Strings.Len(OutFile) - 8;
            // txtFile = Strings.Mid(OutFile, 5, posTxt);
            txtFile = Path.GetFileNameWithoutExtension(OutFile);

            var line = default(string);
            line = FileSystem.LineInput(nFic); // ligne 1 fait rien. Nom d

            line = FileSystem.LineInput(nFic); // ligne 2 donne le nombre de lignes

            try
            {
                DataLength = Conversions.ToInteger(line);
                arrPanne = new StrctPanne[DataLength + 1];
                arrMatrice = new StrctCalc[DataLength + 1];
                arrDaten = new StrctMeteo[DataLength + 1];
            }
            catch
            {
            }

            line = FileSystem.LineInput(nFic); // lire ligne 3

            int MyPos6 = Strings.InStr(1, line, "6"); // recherche des titre des colonnes 
            int MyPos13 = Strings.InStr(1, line, "13");
            int MyPos17 = Strings.InStr(1, line, "17");
            int MyPos22 = Strings.InStr(1, line, "22");
            int MyPos80 = Strings.InStr(1, line, "80");

            if (MyPos80 != 0)
            {
                CasInput = 1; // matriceStrctMeteo avec les colonnes 6,13,17,22,80
            }
            if (MyPos80 == 0)
            {
                CasInput = 2; // matriceStrctMeteo avec les colonnes 6,13,17,22 (sans neige)
            }

            bool bFertig = false;
            int i = 0;
            while (!bFertig)
            {

                try // test s'il y a du text ou pas
                {
                    FileSystem.Input(nFic, ref arrDaten[i].datum);
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
                        FileSystem.Input(nFic, ref arrDaten[i].heure);
                        FileSystem.Input(nFic, ref arrDaten[i].moy6);
                        FileSystem.Input(nFic, ref arrDaten[i].moy13);
                        FileSystem.Input(nFic, ref arrDaten[i].moy17);
                        FileSystem.Input(nFic, ref arrDaten[i].moy22);
                        arrDaten[i].moy80 = float.Parse(FileSystem.LineInput(nFic));
                    }
                    else if (CasInput == 2) // sans arrDaten(i).moy80
                    {
                        FileSystem.Input(nFic, ref arrDaten[i].heure);
                        FileSystem.Input(nFic, ref arrDaten[i].moy6);
                        FileSystem.Input(nFic, ref arrDaten[i].moy13);
                        FileSystem.Input(nFic, ref arrDaten[i].moy17);
                        arrDaten[i].moy22 = float.Parse(FileSystem.LineInput(nFic));
                    }

                    if (arrDaten[i].datum - Conversion.Fix(arrDaten[i].datum / 10000d) * 10000d != 229d)
                    {
                        i = i + 1; // élimination du 29. février
                    }
                }

            }

            iAnzahl = i;
            FileSystem.FileClose(nFic);

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // calcul des dates dans la matrice
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            var loopTo = iAnzahl - 1;
            for (i = 0; i <= loopTo; i++)
            {
                arrMatrice[i].year1 = (int)Math.Round(Conversion.Fix(arrDaten[i].datum / 10000d));
                arrMatrice[i].month = (int)Math.Round(Conversion.Fix((arrDaten[i].datum - 10000 * arrMatrice[i].year1) / 100d));
                arrMatrice[i].day = arrDaten[i].datum - arrMatrice[i].year1 * 10000 - arrMatrice[i].month * 100;
                arrMatrice[i].hour = arrDaten[i].heure;
                arrMatrice[i].year2 = (float)(arrMatrice[i].year1 + arrMatrice[i].month / 12d + arrMatrice[i].day / 366d + arrMatrice[i].hour / (double)(24 * 366));
            }

        }

        public object Troubleshoot(int number)
        {

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // détection des pannes
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            bool Panne = false;
            int NbrPanne = 0;
            int i = 0;

            // //On sauvegarde l'état bout de scotch
            // int iAnzahlOriginal = iAnzahl;
            // StrctMeteo[] arrDatenOriginal = arrDaten;
            // StrctCalc[] arrMatriceOriginal = arrMatrice;
            
            
            var loopTo = iAnzahl - 1;
            for (i = 0; i <= loopTo; i++)
            {
                if (arrDaten[i].moy6 == 32767f & !Panne)
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
                if (arrDaten[i].moy6 != 32767f & Panne)
                {
                    Panne = false;
                    arrPanne[NbrPanne].PanneEnd = i - 1;
                    NbrPanne = NbrPanne + 1;
                }
            }

            var loopTo1 = iAnzahl - 1;
            for (i = 0; i <= loopTo1; i++)
            {
                if (arrDaten[i].moy13 == 32767f & !Panne)
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
                if (arrDaten[i].moy13 != 32767f & Panne)
                {
                    Panne = false;
                    arrPanne[NbrPanne].PanneEnd = i - 1;
                    NbrPanne = NbrPanne + 1;
                }
            }

            var loopTo2 = iAnzahl - 1;
            for (i = 0; i <= loopTo2; i++)
            {
                if (arrDaten[i].moy17 == 32767L & !Panne)
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
                if (arrDaten[i].moy17 != 32767L & Panne)
                {
                    Panne = false;
                    arrPanne[NbrPanne].PanneEnd = i - 1;
                    NbrPanne = NbrPanne + 1;
                }
            }

            var loopTo3 = iAnzahl - 1;
            for (i = 0; i <= loopTo3; i++)
            {
                if (arrDaten[i].moy22 == 32767f & !Panne)
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
                if (arrDaten[i].moy22 != 32767f & Panne)
                {
                    Panne = false;
                    arrPanne[NbrPanne].PanneEnd = i - 1;
                    NbrPanne = NbrPanne + 1;
                }
            }

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // afficher les pannes
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            var MessagePanne = default(string);
            string strDebut;
            string strFin;

            var loopTo4 = NbrPanne - 1;
            for (i = 0; i <= loopTo4; i++)
            {
                strDebut = arrMatrice[arrPanne[i].PanneStart].day + "." + arrMatrice[arrPanne[i].PanneStart].month + "." + arrMatrice[arrPanne[i].PanneStart].year1 + " à " + arrMatrice[arrPanne[i].PanneStart].hour + "H  ";
                strFin = arrMatrice[arrPanne[i].PanneEnd].day + ". " + arrMatrice[arrPanne[i].PanneEnd].month + "." + arrMatrice[arrPanne[i].PanneEnd].year1 + " à " + arrMatrice[arrPanne[i].PanneEnd].hour + "H  ";
                MessagePanne = MessagePanne + "Panne " + (i + 1).ToString() + "  du  " + strDebut + "  au  " + strFin + arrPanne[i].PanneMesure + Constants.vbCrLf;
            }

            if (NbrPanne == 0)
            {
                MessagePanne = "Il n'y a pas de panne dans la série!";
            }
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
            var loopTo5 = iAnzahl - 1;
            for (i = 0; i <= loopTo5; i++) // i correspond à une heure
            {
                if (arrDaten[i].moy6 == 32767f | arrDaten[i].moy13 == 32767f | arrDaten[i].moy17 == 32767L | arrDaten[i].moy22 == 32767f)
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

            if (Panne == false) // contrôle du dernier intervalle
            {
                Fin = i - 1;
                if (Fin - Start > finmax - startmax)
                {
                    finmax = Fin;
                    startmax = Start;
                }
            }

            IntLong = finmax - startmax +1;
            // IntLong = Fix(IntLong / 8760)
            // iAnzahl = CInt(8760 * IntLong)
            iAnzahl = IntLong;

            strDebut = arrMatrice[startmax].day + "." + arrMatrice[startmax].month + "." + arrMatrice[startmax].year1 + " à " + arrMatrice[startmax].hour + "H  ";
            strFin = arrMatrice[startmax + iAnzahl - 1].day + "." + arrMatrice[startmax + iAnzahl - 1].month + "." + arrMatrice[startmax + iAnzahl - 1].year1 + " à " + arrMatrice[startmax + iAnzahl - 1].hour + "H  ";
            // 'MsgBox(" du  " & strDebut & " au  " & strFin, , "Interval maximal sans pannes ")
            string MessageFinaleInterval;
            MessageFinaleInterval = " du  " + strDebut + " au  " + strFin + " " + "Interval maximal sans pannes ";

            NbrAns = iAnzahl / 8760d;

            var loopTo6 = iAnzahl - 1;
            for (i = 0; i <= loopTo6; i++)
                arrMatrice[i] = arrMatrice[i + startmax];
            Array.Resize(ref arrMatrice, iAnzahl);
            var loopTo7 = iAnzahl - 1;
            for (i = 0; i <= loopTo7; i++)
                arrDaten[i] = arrDaten[i + startmax];
            Array.Resize(ref arrDaten, iAnzahl);

            //bout de scotch ARI TURAL
            // arrDaten = arrDatenOriginal;
            // arrMatrice = arrMatriceOriginal;
            // iAnzahl = iAnzahlOriginal;
            
            if (number == 0)
            {
                return MessageFinalePanne;
            }
            else
            {
                return MessageFinaleInterval;
            }

        }

        public object precalcul(string outfile, StrctForm form)
        {
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;
            
            // //BOUT DE SCOTCH ARI TURAL
            // int iAnzahlOriginal = iAnzahl;
            // StrctMeteo[] arrDatenOriginal = arrDaten;
            // StrctCalc[] arrMatriceOriginal = arrMatrice;

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
            var loopTo = iAnzahl - 1;
            for (i = 0; i <= loopTo; i++) // i correspond à une heure
            {
                if (arrDaten[i].moy6 == 32767f | arrDaten[i].moy13 == 32767f | arrDaten[i].moy17 == 32767L | arrDaten[i].moy22 == 32767f)
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

            if (Panne == false) // contrôle du dernier intervalle
            {
                Fin = i - 1;
                if (Fin - Start > finmax - startmax)
                {
                    finmax = Fin;
                    startmax = Start;
                }
            }

            IntLong = finmax - startmax + 1;
            // IntLong = Fix(IntLong / 8760)
            // iAnzahl = CInt(8760 * IntLong)
            iAnzahl = IntLong;
            NbrAns = iAnzahl / 8760d;

            Array.Resize(ref arrMatrice, iAnzahl);
            var loopTo1 = iAnzahl - 1;
            for (i = 0; i <= loopTo1; i++)
                arrDaten[i] = arrDaten[i + startmax];
            Array.Resize(ref arrDaten, iAnzahl);




            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Calcul du nbre d'interventions et de la quantité de sel épandu
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            bool Hiv = true;
            short Cpt = 0;
            float NDH = 0f;



            // Calcul du nombre de jours hivernaux
            for (int j = 0, loopTo2 = iAnzahl - 1; j <= loopTo2; j++)
            {
                if (arrDaten[j].moy6 / 10f > 0f)
                    Hiv = false;
                if (Cpt == 24)
                {
                    if (Hiv == true)
                        NDH = NDH + 1f;
                    Hiv = true;
                    Cpt = 0;
                }
                Cpt = (short)(Cpt + 1);
            }

            // frmTempSeuil = New frmMeteo
            // frmTempSeuil.Label12.Text = NbrAns

            form.nb_annees = (float)NbrAns;

            if (Math.Round(NbrAns, 1) > Math.Round(NbrAns, 0))
            {
                NbrAns = Math.Round(NbrAns, 0) + 1d;
            }
            else
            {
                NbrAns = Math.Round(NbrAns, 0);
            }
            NDH = (float)((double)NDH / NbrAns);  // nombre de jours hivernaux par ans

            float qNaCl1 = (float)(20.83519974d * (double)NDH + 211.3117439d);   // quantité par an en g/m2 de sel déversé sur la chaussée
            float qNaCl2 = (float)(20.83519974d * (double)NDH - 72.9892168d);  // quantité par an en g/m2 de sel déversé sur la chaussée

            // frmTempSeuil.Label3.Text = CInt(qNaCl1)
            form.concentration_annuelle_chlorure_sodium_epandage_mecanique = (int)Math.Round(qNaCl1);
            // frmTempSeuil.Label74.Text = CInt(qNaCl2)
            form.concentration_annuelle_chlorure_sodium_epandage_automatique = (int)Math.Round(qNaCl2);
            // frmTempSeuil.NumericUpDown1.Text = 10

            form.quantite_moyenne_chlorure_epandage_mecanique = 10f;

            form.concentration_chlorure = 0.001f;
            form.epaisseur_film_eau_chaussee = 2f;
            form.humidite_relative_seuil_intervention = 95f;
            form.intervalle_minimal_entre_2 = 8f;
            form.concentration_chlorure_sodium_epandage_mecanique = 36f;
            form.quantite_moyenne_chlorure_epandage_automatique = 0.5f;
            form.nb_giclage_par_intervalle = 12f;
            form.concentration_chlorure_sodium_epandage_automatique = 21f;
            form.position_de_la_1_temperature_exterieur = 1f;
            form.position_de_la_2_temperature_exterieur = 3f;
            form.attenuation_de_1_temperature_exterieur = 1f;
            form.attenuation_de_2_temperature_exterieur = 2f;
            form.difference_de_temperature_exterieur = 100f;
            form.position_de_la_1_humidite_exterieur = 2f;
            form.position_de_la_2_humidite_exterieur = 3f;
            form.attenuation_de_1_humidite_exterieur = 1f;
            form.attenuation_de_2_humidite_exterieur = 4f;
            form.difference_de_humidite_exterieur = 100f;
            form.position_de_la_1_temperature_interieure = 1f;
            form.position_de_la_2_temperature_interieure = 3f;
            form.attenuation_de_1_temperature_interieure = 1f;
            form.attenuation_de_2_temperature_interieure = 8f;
            form.difference_de_temperature_interieure = 100f;
            form.position_de_la_1_humidite_interieure = 2f;
            form.position_de_la_2_humidite_interieure = 3f;
            form.attenuation_de_1_humidite_interieure = 1f;
            form.attenuation_de_2_humidite_interieure = 1f;
            form.difference_de_humidite_interieure = 100f;

            form.nb_intervention_epandage = form.quantite_moyenne_chlorure_epandage_mecanique;


            //BOUT DE SCHOTCH ARI TURAL
            // arrDaten = arrDatenOriginal;
            // arrMatrice = arrMatriceOriginal;
            // iAnzahl = iAnzahlOriginal;



            return form;


        }

        public object InputDeicingSalt(StrctForm form)
        {


            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Calcul du nbre d'interventions et de la quantité de sel épandu
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            bool Hiv = true;
            short Cpt = 0;
            float NDH = 0f;



            // Calcul du nombre de jours hivernaux
            for (int i = 0, loopTo = iAnzahl - 1; i <= loopTo; i++)
            {
                if (arrDaten[i].moy6 / 10f > 0f)
                    Hiv = false;
                if (Cpt == 24)
                {
                    if (Hiv == true)
                        NDH = NDH + 1f;
                    Hiv = true;
                    Cpt = 0;
                }
                Cpt = (short)(Cpt + 1);
            }

            // frmTempSeuil = New frmMeteo
            // frmTempSeuil.Label12.Text = NbrAns

            form.nb_annees = (float)NbrAns;

            if (Math.Round(NbrAns, 1) > Math.Round(NbrAns, 0))
            {
                NbrAns = Math.Round(NbrAns, 0) + 1d;
            }
            else
            {
                NbrAns = Math.Round(NbrAns, 0);
            }
            NDH = (float)((double)NDH / NbrAns);  // nombre de jours hivernaux par ans

            float qNaCl1 = (float)(20.83519974d * (double)NDH + 211.3117439d);   // quantité par an en g/m2 de sel déversé sur la chaussée
            float qNaCl2 = (float)(20.83519974d * (double)NDH - 72.9892168d);  // quantité par an en g/m2 de sel déversé sur la chaussée

            // frmTempSeuil.Label3.Text = CInt(qNaCl1)
            form.concentration_annuelle_chlorure_sodium_epandage_mecanique = (int)Math.Round(qNaCl1);
            // frmTempSeuil.Label74.Text = CInt(qNaCl2)
            form.concentration_annuelle_chlorure_sodium_epandage_automatique = (int)Math.Round(qNaCl2);
            // frmTempSeuil.NumericUpDown1.Text = 10

            form.quantite_moyenne_chlorure_epandage_mecanique = 10f;

            // frmTempSeuil.ButtonExportFile.Hide()
            // frmTempSeuil.ButtonExportDB.Hide()
            // frmTempSeuil.LabelOR.Hide()
            // frmTempSeuil.ShowDialog()
            // frmTempSeuil.Hide()

            // calcul de la concentration en NaCl dans l'eau
            // Dim DureeInt As Short = frmTempSeuil.NumericUpDown2.Text
            short DureeInt = (short)Math.Round(form.intervalle_minimal_entre_2);
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
            float EpNa1 = form.concentration_chlorure_sodium_epandage_mecanique / 100f;
            // Dim EpNa2 As Single = frmTempSeuil.NumericUpDown23.Text / 100
            float EpNa2 = form.concentration_chlorure_sodium_epandage_automatique / 100f;
            // Dim Feau As Single = frmTempSeuil.NumericUpDown5.Text
            float Feau = form.epaisseur_film_eau_chaussee;

            short Dint1 = 0;
            short Dint2 = 0;
            bool PluieOld = false;

            for (int i = 0, loopTo1 = iAnzahl - 1; i <= loopTo1; i++)
            {
                if (Dint1 != 0)
                    Dint1 = (short)(Dint1 + 1);
                if (Dint2 != 0)
                    Dint2 = (short)(Dint2 + 1);
                if (Dint1 >= DureeInt)
                    Dint1 = 0;
                if (Dint1 == 0 & arrDaten[i].moy6 / 10f < Tseuil1 & (arrDaten[i].moy13 / 10f >= HRseuil | arrDaten[i].moy17 / 10d > 0d)) // why or
                {
                    if (arrDaten[i].moy17 / 10d == 0d) // we don't have rain now
                    {
                        arrMatrice[i].salage1 = EpNa1.ToString();
                    }
                    else if (PluieOld == false) // we have rain now
                                                // we didn't have rain the step before
                    {
                        arrMatrice[i].salage1 = ((double)QNa1 / (1000L * arrDaten[i].moy17 / 10d)).ToString();
                    }
                    else if (i != 0) // we had rain the step before
                        arrMatrice[i].salage1 = ((Conversions.ToDouble(arrMatrice[i - 1].salage1) * 1000d * (double)Feau + (double)QNa1) / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    Dint1 = (short)(Dint1 + 1);
                }
                if (Dint2 == 0 & arrDaten[i].moy6 / 10f < Tseuil2 & (arrDaten[i].moy13 / 10f >= HRseuil | arrDaten[i].moy17 / 10d > 0d))
                {
                    if (arrDaten[i].moy17 / 10d == 0d)
                    {
                        arrMatrice[i].salage2 = EpNa2.ToString();
                    }
                    else if (PluieOld == false)
                    {
                        arrMatrice[i].salage2 = ((double)QNa2 / (1000L * arrDaten[i].moy17 / 10d)).ToString();
                    }
                    else if (i != 0)

                        arrMatrice[i].salage2 = ((Conversions.ToDouble(arrMatrice[i - 1].salage2) * 1000d * (double)Feau + (double)QNa2) / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    Dint2 = (short)(Dint2 + 1);
                }
                if (arrDaten[i].moy17 / 10d != 0d)
                {
                    PluieOld = true;
                }
                else
                {
                    PluieOld = false;
                }
                if (Dint1 != 1)
                {
                    if (PluieOld == true)
                    {
                        if (i != 0)
                            arrMatrice[i].salage1 = (Conversions.ToDouble(arrMatrice[i - 1].salage1) * 1000d * (double)Feau / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    }
                    else if (i != 0) // PluieOld = False
                        arrMatrice[i].salage1 = arrMatrice[i - 1].salage1;
                    // If i = 0 Then arrMatrice(i).salage1 = frmTempSeuil.NumericUpDown6.Value / 100
                    if (i == 0)
                        arrMatrice[i].salage1 = (form.concentration_chlorure / 100f).ToString();
                }
                if (Dint2 != 1)
                {
                    if (PluieOld == true)
                    {
                        if (i > 0)
                            arrMatrice[i].salage2 = (Conversions.ToDouble(arrMatrice[i - 1].salage2) * 1000d * (double)Feau / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    }
                    else if (i > 0)
                        arrMatrice[i].salage2 = arrMatrice[i - 1].salage2;
                    // If i = 0 Then arrMatrice(i).salage2 = frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Text / 100
                    if (i == 0)
                        arrMatrice[i].salage2 = (form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle / 100f).ToString();
                    if (Conversions.ToDouble(arrMatrice[i].salage2) <= 0.1d * (double)EpNa2)
                        Dint2 = 0; // ???
                }
                if (Conversions.ToDouble(arrMatrice[i].salage1) > (double)EpNa1)
                    arrMatrice[i].salage1 = EpNa1.ToString(); // keep the maximal value
                if (Conversions.ToDouble(arrMatrice[i].salage2) > (double)EpNa2)
                    arrMatrice[i].salage2 = EpNa2.ToString();
            }
            return form;

        }
        public void ExportFile(StrctForm Form)
        {



            // calcul de la concentration en NaCl dans l'eau
            // Dim DureeInt As Short = frmTempSeuil.NumericUpDown2.Text
            short DureeInt = (short)Math.Round(Form.intervalle_minimal_entre_2);
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
            // DExportFileim EpNa1 As Single = frmTempSeuil.NumericUpDown4.Text / 100
            float EpNa1 = Form.concentration_chlorure_sodium_epandage_mecanique / 100f;
            // Dim EpNa2 As Single = frmTempSeuil.NumericUpDown23.Text / 100
            float EpNa2 = Form.concentration_chlorure_sodium_epandage_automatique / 100f;
            // Dim Feau As Single = frmTempSeuil.NumericUpDown5.Text
            float Feau = Form.epaisseur_film_eau_chaussee;

            short Dint1 = 0;
            short Dint2 = 0;
            bool PluieOld = false;

            for (int i = 0, loopTo = iAnzahl - 1; i <= loopTo; i++)
            {
                if (Dint1 != 0)
                    Dint1 = (short)(Dint1 + 1);
                if (Dint2 != 0)
                    Dint2 = (short)(Dint2 + 1);
                if (Dint1 >= DureeInt)
                    Dint1 = 0;
                if (Dint1 == 0 & arrDaten[i].moy6 / 10f < Tseuil1 & (arrDaten[i].moy13 / 10f >= HRseuil | arrDaten[i].moy17 / 10d > 0d)) // why or
                {
                    if (arrDaten[i].moy17 / 10d == 0d) // we don't have rain now
                    {
                        arrMatrice[i].salage1 = EpNa1.ToString();
                    }
                    else if (PluieOld == false) // we have rain now
                                                // we didn't have rain the step before
                    {
                        arrMatrice[i].salage1 = ((double)QNa1 / (1000L * arrDaten[i].moy17 / 10d)).ToString();
                    }
                    else if (i != 0) // we had rain the step before
                        arrMatrice[i].salage1 = ((Conversions.ToDouble(arrMatrice[i - 1].salage1) * 1000d * (double)Feau + (double)QNa1) / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    Dint1 = (short)(Dint1 + 1);
                }
                if (Dint2 == 0 & arrDaten[i].moy6 / 10f < Tseuil2 & (arrDaten[i].moy13 / 10f >= HRseuil | arrDaten[i].moy17 / 10d > 0d))
                {
                    if (arrDaten[i].moy17 / 10d == 0d)
                    {
                        arrMatrice[i].salage2 = EpNa2.ToString();
                    }
                    else if (PluieOld == false)
                    {
                        arrMatrice[i].salage2 = ((double)QNa2 / (1000L * arrDaten[i].moy17 / 10d)).ToString();
                    }
                    else if (i != 0)

                        arrMatrice[i].salage2 = ((Conversions.ToDouble(arrMatrice[i - 1].salage2) * 1000d * (double)Feau + (double)QNa2) / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    Dint2 = (short)(Dint2 + 1);
                }
                if (arrDaten[i].moy17 / 10d != 0d)
                {
                    PluieOld = true;
                }
                else
                {
                    PluieOld = false;
                }
                if (Dint1 != 1)
                {
                    if (PluieOld == true)
                    {
                        if (i != 0)
                            arrMatrice[i].salage1 = (Conversions.ToDouble(arrMatrice[i - 1].salage1) * 1000d * (double)Feau / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    }
                    else if (i != 0) // PluieOld = False
                        arrMatrice[i].salage1 = arrMatrice[i - 1].salage1;
                    // If i = 0 Then arrMatrice(i).salage1 = frmTempSeuil.NumericUpDown6.Value / 100
                    if (i == 0)
                        arrMatrice[i].salage1 = (Form.concentration_chlorure / 100f).ToString();
                }
                if (Dint2 != 1)
                {
                    if (PluieOld == true)
                    {
                        if (i > 0)
                            arrMatrice[i].salage2 = (Conversions.ToDouble(arrMatrice[i - 1].salage2) * 1000d * (double)Feau / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                    }
                    else if (i > 0)
                        arrMatrice[i].salage2 = arrMatrice[i - 1].salage2;
                    // If i = 0 Then arrMatrice(i).salage2 = frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Text / 100
                    if (i == 0)
                        arrMatrice[i].salage2 = (Form.quantite_moyenne_chlorure_epandage_automatique * Form.nb_giclage_par_intervalle / 100f).ToString();
                    if (Conversions.ToDouble(arrMatrice[i].salage2) <= 0.1d * (double)EpNa2)
                        Dint2 = 0; // ???
                }
                if (Conversions.ToDouble(arrMatrice[i].salage1) > (double)EpNa1)
                    arrMatrice[i].salage1 = EpNa1.ToString(); // keep the maximal value
                if (Conversions.ToDouble(arrMatrice[i].salage2) > (double)EpNa2)
                    arrMatrice[i].salage2 = EpNa2.ToString();
            }
            CalculTHS(Form);
        }
        public void CalculTHS(StrctForm form)
        {
            // var form = default(StrctForm);
            var InputMatrice = new float[DataLength + 1];
            var OutputMatrice = new float[DataLength + 1];

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // calcul  T et Ts
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            float a = 0.7f;
            float hy = 20f;

            for (int i = 0, loopTo = iAnzahl - 1; i <= loopTo; i++)
            {
                arrMatrice[i].T = arrDaten[i].moy6 / 10f;
                if (arrDaten[i].moy22 < 0f)
                {
                    arrDaten[i].moy22 = 0f;
                }
                arrMatrice[i].Ts = arrMatrice[i].T + a / hy * arrDaten[i].moy22;
            }

            for (int i = 0, loopTo1 = iAnzahl - 1; i <= loopTo1; i++)    // calcul de Text
                InputMatrice[i] = arrMatrice[i].T;

            // AttenBruit(CSng(frmTempSeuil.NumericUpDown8.Value), CSng(frmTempSeuil.NumericUpDown7.Value), CSng(frmTempSeuil.NumericUpDown9.Value), CSng(frmTempSeuil.NumericUpDown10.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox1.Text))
            float argA = form.position_de_la_1_temperature_exterieur;
            float argB = form.position_de_la_2_temperature_exterieur;
            float argC = form.attenuation_de_1_temperature_exterieur;
            float argD = form.attenuation_de_2_temperature_exterieur;
            float argtlim = form.difference_de_temperature_exterieur;
            AttenBruit(ref argA, ref argB, ref argC, ref argD, ref InputMatrice, ref OutputMatrice, ref argtlim);
            for (int i = 0, loopTo2 = iAnzahl - 1; i <= loopTo2; i++)
                arrMatrice[i].Text = OutputMatrice[i];

            arrMatrice[iAnzahl - 1].Text = InputMatrice[iAnzahl - 1];

            for (int i = 0, loopTo3 = iAnzahl - 1; i <= loopTo3; i++)    // calcul de Tcaisson
                InputMatrice[i] = arrMatrice[i].T;

            // AttenBruit(CSng(frmTempSeuil.NumericUpDown21.Value), CSng(frmTempSeuil.NumericUpDown22.Value), CSng(frmTempSeuil.NumericUpDown19.Value), CSng(frmTempSeuil.NumericUpDown20.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox4.Text))
            float argA1 = form.position_de_la_1_temperature_interieure;
            float argB1 = form.position_de_la_2_temperature_interieure;
            float argC1 = form.attenuation_de_1_temperature_interieure;
            float argD1 = form.attenuation_de_2_temperature_interieure;
            float argtlim1 = form.difference_de_temperature_interieure;
            AttenBruit(ref argA1, ref argB1, ref argC1, ref argD1, ref InputMatrice, ref OutputMatrice, ref argtlim1);

            for (int i = 0, loopTo4 = iAnzahl - 1; i <= loopTo4; i++)
                arrMatrice[i].Tcaisson = OutputMatrice[i];
            arrMatrice[iAnzahl - 1].Tcaisson = InputMatrice[iAnzahl - 1];

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // calculs d'exposition HR
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// 
            short NbPluie = 0; // Ajout Bitume TSANCHEZ
            short NbPluieMax = 50; // valeu par défaut

            for (int i = 0, loopTo5 = iAnzahl - 1; i <= loopTo5; i++)
            {

                if (arrDaten[i].moy13 >= 1000f) // exposition brouillard, pas de pluie
                {
                    arrMatrice[i].HR_brouillard = 99.99f;
                    arrMatrice[i].HR_bitume = arrMatrice[i].HR_brouillard; // Ajout Bitume TSANCHEZ
                }
                else
                {
                    arrMatrice[i].HR_brouillard = arrDaten[i].moy13 / 10f;
                    arrMatrice[i].HR_bitume = arrMatrice[i].HR_brouillard;
                } // Ajout Bitume TSANCHEZ

                if (i > 0) // exposition eclaboussures
                {
                    if (arrDaten[i].moy17 != 0L & arrDaten[i - 1].moy17 != 0L) // pluie avant une heure
                    {
                        arrMatrice[i].HR_eclaboussures = 100f;
                    }
                    else if (arrDaten[i].moy13 >= 1000f) // pas de pluie
                    {
                        arrMatrice[i].HR_eclaboussures = 99.99f;
                    }
                    else
                    {
                        arrMatrice[i].HR_eclaboussures = arrDaten[i].moy13 / 10f;
                    }
                }

                if (arrMatrice[i].hour > 17 | arrMatrice[i].hour < 6) // exposition stagnation (direct)
                {
                    // pendant la nuit (de 18h00 à 6h00)
                    if (arrDaten[i].moy17 != 0L) // pluie
                    {
                        arrMatrice[i].HR_direct = 100f;
                        NbPluie = (short)(NbPluie + 1);
                        if (NbPluie == NbPluieMax)                // Ajout Bitume TSANCHEZ
                        {
                            arrMatrice[i].HR_bitume = 100f;
                            NbPluie = 0;
                        }
                    }
                    else if (arrDaten[i].moy13 >= 1000f) // pas de pluie
                    {
                        arrMatrice[i].HR_direct = 99.99f;
                    }
                    else
                    {
                        arrMatrice[i].HR_direct = arrDaten[i].moy13 / 10f;
                    }
                }
                else if (arrDaten[i].moy17 != 0L) // pluie
                {
                    arrMatrice[i].HR_direct = 100f;
                }
                else if (arrDaten[i].moy13 >= 1000f) // pas de pluie
                {
                    arrMatrice[i].HR_direct = 99.99f;
                }
                else
                {
                    arrMatrice[i].HR_direct = arrDaten[i].moy13 / 10f;
                }
            }

            for (int i = 0, loopTo6 = iAnzahl - 1; i <= loopTo6; i++)    // calcul de HRext
                InputMatrice[i] = arrMatrice[i].HR_brouillard;
            // AttenBruit(CSng(frmTempSeuil.NumericUpDown13.Value), CSng(frmTempSeuil.NumericUpDown14.Value), CSng(frmTempSeuil.NumericUpDown11.Value), CSng(frmTempSeuil.NumericUpDown12.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox2.Text))
            float argA2 = form.position_de_la_1_humidite_exterieur;
            float argB2 = form.position_de_la_2_humidite_exterieur;
            float argC2 = form.attenuation_de_1_humidite_exterieur;
            float argD2 = form.attenuation_de_2_humidite_exterieur;
            float argtlim2 = form.difference_de_humidite_exterieur;
            AttenBruit(ref argA2, ref argB2, ref argC2, ref argD2, ref InputMatrice, ref OutputMatrice, ref argtlim2);

            for (int i = 0, loopTo7 = iAnzahl - 1; i <= loopTo7; i++)
                arrMatrice[i].HR_ext = OutputMatrice[i];
            arrMatrice[iAnzahl - 1].HR_ext = InputMatrice[iAnzahl - 1];

            for (int i = 0, loopTo8 = iAnzahl - 1; i <= loopTo8; i++)    // calcul de HRcaisson
                InputMatrice[i] = arrMatrice[i].HR_brouillard;
            // AttenBruit(CSng(frmTempSeuil.NumericUpDown17.Value), CSng(frmTempSeuil.NumericUpDown18.Value), CSng(frmTempSeuil.NumericUpDown15.Value), CSng(frmTempSeuil.NumericUpDown16.Value), InputMatrice, OutputMatrice, CSng(frmTempSeuil.TextBox3.Text))
            float argA3 = form.position_de_la_1_humidite_interieure;
            float argB3 = form.position_de_la_2_humidite_interieure;
            float argC3 = form.attenuation_de_1_humidite_interieure;
            float argD3 = form.attenuation_de_2_humidite_interieure;
            float argtlim3 = form.difference_de_humidite_interieure;
            AttenBruit(ref argA3, ref argB3, ref argC3, ref argD3, ref InputMatrice, ref OutputMatrice, ref argtlim3);

            for (int i = 0, loopTo9 = iAnzahl - 1; i <= loopTo9; i++)
                arrMatrice[i].HR_caisson = OutputMatrice[i];
            arrMatrice[iAnzahl - 1].HR_caisson = InputMatrice[iAnzahl - 1];

        }
        
        private void WriteExpoFileData(TextWriter writer, Meteo_File[] data, int count)
        {
            writer.WriteLine(count);
            writer.WriteLine("3600");

            for (int i = 0; i < count; i++)
            {
                string line = data[i].HR.ToString(CultureInfo.InvariantCulture)
                              + Constants.vbTab + Constants.vbTab
                              + data[i].Sel.ToString(CultureInfo.InvariantCulture)
                              + Constants.vbTab + Constants.vbTab
                              + data[i].Tsurf.ToString(CultureInfo.InvariantCulture);

                writer.WriteLine(line);
            }

            writer.Close();
        }


        public void WriteExpoFile(ref string OutFile, ref string PostFile, ref string txtFile, ref bool Canc)
        {

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // création des fichiers INPUT
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            TextWriter INFile1, INFile2, INFile3, INFile4, INFile5, INFile6, INFile7, INFile8, INFile9, INFile10, INFile11, INFile12, INFile13, INFile14, INFile15, INFile16, INFile17, INFile18;
            String exportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../public/exports/");
            
            //CREATION DU DOSSIER DE SERIE D EXPOSITION
            PostFile = Path.Combine(exportFolder, txtFile + '/');
            
            Directory.CreateDirectory(PostFile);
            
            OutFile = PostFile + "EXPO_M_E_E_" + txtFile + ".txt";
            INFile1 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_E_O_" + txtFile + ".txt";
            INFile2 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_B_E_" + txtFile + ".txt";
            INFile3 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_B_O_" + txtFile + ".txt";
            INFile4 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_D_E_" + txtFile + ".txt";
            INFile5 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_D_O_" + txtFile + ".txt";
            INFile6 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_EXT_" + txtFile + ".txt";
            INFile7 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_CAI_" + txtFile + ".txt";
            INFile8 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_CAC_" + txtFile + ".txt";
            INFile9 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_E_E_" + txtFile + ".txt";
            INFile10 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_E_O_" + txtFile + ".txt";
            INFile11 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_B_E_" + txtFile + ".txt";
            INFile12 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_B_O_" + txtFile + ".txt";
            INFile13 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_D_E_" + txtFile + ".txt";
            INFile14 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_D_O_" + txtFile + ".txt";
            INFile15 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_EXT_" + txtFile + ".txt";
            INFile16 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_A_CAC_" + txtFile + ".txt";
            INFile17 = File.CreateText(OutFile);
            OutFile = PostFile + "EXPO_M_BIT_" + txtFile + ".txt";
            INFile18 = File.CreateText(OutFile);

            var arrINPUT_M_E_E = new Meteo_File[iAnzahl];
            var arrINPUT_M_E_O = new Meteo_File[iAnzahl];
            var arrINPUT_M_B_E = new Meteo_File[iAnzahl];
            var arrINPUT_M_B_O = new Meteo_File[iAnzahl];
            var arrINPUT_M_D_E = new Meteo_File[iAnzahl];
            var arrINPUT_M_D_O = new Meteo_File[iAnzahl];
            var arrINPUT_M_EXT = new Meteo_File[iAnzahl];
            var arrINPUT_M_CAI = new Meteo_File[iAnzahl];
            var arrINPUT_M_CAC = new Meteo_File[iAnzahl];
            var arrINPUT_A_E_E = new Meteo_File[iAnzahl];
            var arrINPUT_A_E_O = new Meteo_File[iAnzahl];
            var arrINPUT_A_B_E = new Meteo_File[iAnzahl];
            var arrINPUT_A_B_O = new Meteo_File[iAnzahl];
            var arrINPUT_A_D_E = new Meteo_File[iAnzahl];
            var arrINPUT_A_D_O = new Meteo_File[iAnzahl];
            var arrINPUT_A_EXT = new Meteo_File[iAnzahl];
            var arrINPUT_A_CAC = new Meteo_File[iAnzahl];
            var arrINPUT_M_BIT = new Meteo_File[iAnzahl];

            for (int i = 0, loopTo = iAnzahl - 1; i <= loopTo; i++)
            {
                // eclaboussure et ensoleillement
                arrINPUT_M_E_E[i].HR = arrMatrice[i].HR_eclaboussures;
                arrINPUT_M_E_E[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_E_E[i].Tsurf = arrMatrice[i].Ts;
                // eclaboussure et ombrée
                arrINPUT_M_E_O[i].HR = arrMatrice[i].HR_eclaboussures;
                arrINPUT_M_E_O[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_E_O[i].Tsurf = arrMatrice[i].T;
                // brouillard et ensoleillement
                arrINPUT_M_B_E[i].HR = arrMatrice[i].HR_brouillard;
                arrINPUT_M_B_E[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_B_E[i].Tsurf = arrMatrice[i].Ts;
                // brouillard et ombrée
                arrINPUT_M_B_O[i].HR = arrMatrice[i].HR_brouillard;
                arrINPUT_M_B_O[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_B_O[i].Tsurf = arrMatrice[i].T;
                // direct et ensoleillement
                arrINPUT_M_D_E[i].HR = arrMatrice[i].HR_direct;
                arrINPUT_M_D_E[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_D_E[i].Tsurf = arrMatrice[i].Ts;
                // direct et ombrée
                arrINPUT_M_D_O[i].HR = arrMatrice[i].HR_direct;
                arrINPUT_M_D_O[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_D_O[i].Tsurf = arrMatrice[i].T;
                // extérieur et à l'abris des intempéries
                arrINPUT_M_EXT[i].HR = arrMatrice[i].HR_ext;
                arrINPUT_M_EXT[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_EXT[i].Tsurf = arrMatrice[i].Text;
                // intérieur du caisson et sans sel
                arrINPUT_M_CAI[i].HR = arrMatrice[i].HR_caisson;
                arrINPUT_M_CAI[i].Sel = 0f;
                arrINPUT_M_CAI[i].Tsurf = arrMatrice[i].Tcaisson;
                // intérieur du caisson et avec présence de sel
                arrINPUT_M_CAC[i].HR = arrMatrice[i].HR_caisson;
                arrINPUT_M_CAC[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_CAC[i].Tsurf = arrMatrice[i].Tcaisson;
                // Statgnant avec Bitume TSANCHEZ
                arrINPUT_M_BIT[i].HR = arrMatrice[i].HR_bitume;
                arrINPUT_M_BIT[i].Sel = Conversions.ToSingle(arrMatrice[i].salage1);
                arrINPUT_M_BIT[i].Tsurf = arrMatrice[i].T;

                // eclaboussure et ensoleillement
                arrINPUT_A_E_E[i].HR = arrMatrice[i].HR_eclaboussures;
                arrINPUT_A_E_E[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_E_E[i].Tsurf = arrMatrice[i].Ts;
                // eclaboussure et ombrée
                arrINPUT_A_E_O[i].HR = arrMatrice[i].HR_eclaboussures;
                arrINPUT_A_E_O[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_E_O[i].Tsurf = arrMatrice[i].T;
                // brouillard et ensoleillement
                arrINPUT_A_B_E[i].HR = arrMatrice[i].HR_brouillard;
                arrINPUT_A_B_E[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_B_E[i].Tsurf = arrMatrice[i].Ts;
                // brouillard et ombrée
                arrINPUT_A_B_O[i].HR = arrMatrice[i].HR_brouillard;
                arrINPUT_A_B_O[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_B_O[i].Tsurf = arrMatrice[i].T;
                // direct et ensoleillement
                arrINPUT_A_D_E[i].HR = arrMatrice[i].HR_direct;
                arrINPUT_A_D_E[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_D_E[i].Tsurf = arrMatrice[i].Ts;
                // direct et ombrée
                arrINPUT_A_D_O[i].HR = arrMatrice[i].HR_direct;
                arrINPUT_A_D_O[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_D_O[i].Tsurf = arrMatrice[i].T;
                // extérieur et à l'abris des intempéries
                arrINPUT_A_EXT[i].HR = arrMatrice[i].HR_ext;
                arrINPUT_A_EXT[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_EXT[i].Tsurf = arrMatrice[i].Text;
                // intérieur du caisson et avec présence de sel
                arrINPUT_A_CAC[i].HR = arrMatrice[i].HR_caisson;
                arrINPUT_A_CAC[i].Sel = Conversions.ToSingle(arrMatrice[i].salage2);
                arrINPUT_A_CAC[i].Tsurf = arrMatrice[i].Tcaisson;
            }

            // écriture dans les fichiers
            WriteExpoFileData(INFile1, arrINPUT_M_E_E, iAnzahl);
            WriteExpoFileData(INFile2, arrINPUT_M_E_O, iAnzahl);
            WriteExpoFileData(INFile3, arrINPUT_M_B_E, iAnzahl);
            WriteExpoFileData(INFile4, arrINPUT_M_B_O, iAnzahl);
            WriteExpoFileData(INFile5, arrINPUT_M_D_E, iAnzahl);
            WriteExpoFileData(INFile6, arrINPUT_M_D_O, iAnzahl);
            WriteExpoFileData(INFile7, arrINPUT_M_EXT, iAnzahl);
            WriteExpoFileData(INFile8, arrINPUT_M_CAI, iAnzahl);
            WriteExpoFileData(INFile9, arrINPUT_M_CAC, iAnzahl);
            WriteExpoFileData(INFile10, arrINPUT_A_E_E, iAnzahl);
            WriteExpoFileData(INFile11, arrINPUT_A_E_O, iAnzahl);
            WriteExpoFileData(INFile12, arrINPUT_A_B_E, iAnzahl);
            WriteExpoFileData(INFile13, arrINPUT_A_B_O, iAnzahl);
            WriteExpoFileData(INFile14, arrINPUT_A_D_E, iAnzahl);
            WriteExpoFileData(INFile15, arrINPUT_A_D_O, iAnzahl);
            WriteExpoFileData(INFile16, arrINPUT_A_EXT, iAnzahl);
            WriteExpoFileData(INFile17, arrINPUT_A_CAC, iAnzahl);
            WriteExpoFileData(INFile18, arrINPUT_M_BIT, iAnzahl);
            
            //CREATION D'UN ZIP CONTENANT LES EXPOSITIONS

            String zipName = Path.Combine(exportFolder, $"{txtFile}.zip");
            
            if (File.Exists(zipName))
            {
                File.Delete(zipName);  // DELETE DU ZIP SI IL EXISTE DEJA POUR EVITER LES CRASHS (SUR SYMFONY ON GERE DE MANIERE A CE QU'AUCUN
                                       // FICHIER AIT LE MEME NOM DONC CA NE DEVRAIT PAS ARRIVER
            }
            
            ZipFile.CreateFromDirectory(PostFile,zipName, CompressionLevel.Fastest, false);
            
            //SUPRESSION DU DOSSIER CONTENANT LES EXPOSITIONS
            if (Directory.Exists(PostFile))
            {
                Directory.Delete(PostFile,true);
            }

            PostFile = zipName; //Valeur en référence et qui va donc ressortir de la fonction
        }

        public object MeteoTreatmentTroubleshootingPart1(string outfile)
        {
            // Dim outfile As String
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;

            ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);
            string Retour = Conversions.ToString(Troubleshoot(0));
            return Retour;

        }
        public object MeteoTreatmentTroubleshootingPart2(string outfile)
        {
            // Dim outfile As String
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;

            ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);
            string Retour = Conversions.ToString(Troubleshoot(1));
            return Retour;
        }

        public object MeteoTreatmentInputDeicingSalt(string outfile)
        {
            // Dim outfile As String
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;

            ReadMeteoFile(outfile, ref PostFile, ref txtfile, ref Canc);
            var Form = default(StrctForm);
            InputDeicingSalt(Form);
            return default;
        }

        public object MeteoTreatmentPrecalcul(string outfile)
        {
            var Form = default(StrctForm);
            Form = (StrctForm)precalcul(outfile, Form);
            // string Fichier = @"/tmp/TempSeuil.txt";
            string fichier = Path.Combine(Path.GetTempPath(), "TempSeuil_" + Guid.NewGuid().ToString() + ".txt");
            WriteMeteoFormToTextFile(fichier, Form, false);
            return fichier;

        }
        public void MeteoTreatment()
        {

            var outfile = default(string);
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;

            // ReadMeteoFile(outfile, PostFile, txtfile, Canc)
            // If Canc = True Then End

            // Troubleshoot()

            // InputDeicingSalt()

            CalculTHS(new StrctForm());

            WriteExpoFile(ref outfile, ref PostFile, ref txtfile, ref Canc);

        }
        public object MeteoTreatmentCalcul(List<string> outfiles)
        {
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;
            ReadMeteoFile(outfiles[0], ref PostFile, ref txtfile, ref Canc);
            Troubleshoot(-1);
            var Form = default(StrctForm);
            Form = (StrctForm)InputDeicingSalt(Form);
            Form = (StrctForm)precalcul(outfiles[0], Form);
            Form = (StrctForm)WCal(outfiles[1], Form);
            // string Fichier = @"/tmp/TempSeuil.txt";
            string fichier = Path.Combine(Path.GetTempPath(), "TempSeuil_" + Guid.NewGuid().ToString() + ".txt");
            WriteMeteoFormToTextFile(fichier, Form, true);
            return fichier;
        }

        public object MeteoTreatmentExport(List<string> outfiles)
        {
            var PostFile = default(string);
            var txtfile = default(string);
            bool Canc = false;
            var form = new StrctForm();
            ReadMeteoFile(outfiles[0], ref PostFile, ref txtfile, ref Canc);
            Troubleshoot(-1);
            form = ReadMeteoFormFromTextFile(outfiles[1]);
            ExportFile(form);
            var tmp = outfiles;
            string argOutFile = tmp[1];
            WriteExpoFile(ref argOutFile, ref PostFile, ref txtfile, ref Canc);
            tmp[1] = argOutFile;

            //ON RETOURNE LE CHEMIN VERS LE ZIP
            return PostFile;
        }
        public object WCal(string @file, StrctForm form)
        {
            double NbrAns; // [-]
            int i; // [-]
            int DureeIntrvent; // [h]
            long nbrInt1 = 0L;
            long nbrInt2 = 0L;
            float Tseuil1 = -9;  // °C
            float Tseuil2 = -9;  // °C
            float HRseuil = 0f;
            long Nint1 = 0L;
            long Nint2 = 0L;
            short Dint = 0;
            bool PluieOld = false;
            float QNa = 0f;
            float EpNa = 0f;
            float Feau;
            string Na;

            CalNeige();

            // DureeIntrvent = frmTempSeuil.NumericUpDown2.Value
            DureeIntrvent = (int)Math.Round(form.intervalle_minimal_entre_2);
            // nbrInt1 = CInt(frmTempSeuil.Label3.Text / frmTempSeuil.NumericUpDown1.Value)
            nbrInt1 = (int)Math.Round(form.concentration_annuelle_chlorure_sodium_epandage_mecanique / form.quantite_moyenne_chlorure_epandage_mecanique);
            // nbrInt2 = CInt(frmTempSeuil.Label74.Text / (frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Value))
            nbrInt2 = (int)Math.Round(form.concentration_annuelle_chlorure_sodium_epandage_automatique / (form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle));
            // frmTempSeuil.Label6.Text = nbrInt1
            form.nb_intervention_epandage = nbrInt1;
            // frmTempSeuil.Label76.Text = nbrInt2
            form.nb_giclages_annuel = nbrInt2;
            // NbrAns = frmTempSeuil.Label12.Text
            NbrAns = form.nb_annees;
            nbrInt1 = (int)Math.Round(nbrInt1 * NbrAns);
            nbrInt2 = (int)Math.Round(nbrInt2 * NbrAns);
            // HRseuil = frmTempSeuil.NumericUpDown3.Value
            HRseuil = form.humidite_relative_seuil_intervention;
            while (Nint1 < nbrInt1)
            {
                Nint1 = 0L;
                Dint = 0;
                var loopTo = iAnzahl - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    if (Dint != 0)
                        Dint = (short)(Dint + 1);
                    if (Dint >= DureeIntrvent)
                        Dint = 0;
                    if (Dint == 0 & arrDaten[i].moy6 / 10f < Tseuil1 & (arrDaten[i].moy13 / 10f >= HRseuil | arrDaten[i].moy17 / 10d > 0d))
                    {
                        Nint1 += 1L;
                        Dint = (short)(Dint + 1);
                    }
                }
                Tseuil1 = (float)(Tseuil1 + 0.1d);
            }
            // frmTempSeuil.Label22.Text = CInt(Tseuil1 * 10) / 10
            form.temperature_seuil_epandage_mecanique = (float)((int)Math.Round(Tseuil1 * 10f) / 10d);
            while (Nint2 < nbrInt2)
            {
                Nint2 = 0L;
                Dint = 0;
                Na = 0.ToString();
                // EpNa = frmTempSeuil.NumericUpDown23.Text / 100
                EpNa = form.concentration_chlorure_sodium_epandage_automatique / 100f;
                // QNa = frmTempSeuil.NumericUpDown24.Text * frmTempSeuil.NumericUpDown25.Text
                QNa = form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle;
                PluieOld = false;
                // Feau = frmTempSeuil.NumericUpDown5.Text
                Feau = form.epaisseur_film_eau_chaussee;
                var loopTo1 = iAnzahl - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    if (Dint != 0)
                        Dint = (short)(Dint + 1);
                    if (Dint == 0 & arrDaten[i].moy6 / 10f < Tseuil2 & (arrDaten[i].moy13 / 10f >= HRseuil | arrDaten[i].moy17 / 10d > 0d))
                    {
                        if (arrDaten[i].moy17 / 10d == 0d)
                        {
                            Na = EpNa.ToString();
                        }
                        else if (PluieOld == false)
                        {
                            Na = ((double)QNa / (1000L * arrDaten[i].moy17 / 10d)).ToString();
                        }
                        else if (i != 0)
                            Na = ((Conversions.ToDouble(Na) * 1000d * (double)Feau + (double)QNa) / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                        Dint = (short)(Dint + 1);
                        Nint2 = Nint2 + 1L;
                    }
                    if (arrDaten[i].moy17 / 10d != 0d)
                    {
                        PluieOld = true;
                    }
                    else
                    {
                        PluieOld = false;
                    }
                    if (Dint != 1)
                    {
                        if (PluieOld == true)
                        {
                            if (i > 0)
                                Na = (Conversions.ToDouble(Na) * 1000d * (double)Feau / (((double)Feau + arrDaten[i].moy17 / 10d) * 1000d)).ToString();
                        }
                        // If i = 0 Then Na = frmTempSeuil.NumericUpDown24.Value * frmTempSeuil.NumericUpDown25.Text / 100
                        if (i == 0)
                            Na = (form.quantite_moyenne_chlorure_epandage_automatique * form.nb_giclage_par_intervalle / 100f).ToString();
                        if (Conversions.ToDouble(Na) <= 0.1d * (double)EpNa)
                            Dint = 0;
                    }
                    if (Conversions.ToDouble(Na) > (double)EpNa)
                        Na = EpNa.ToString();
                }
                Tseuil2 = (float)((double)Tseuil2 + 0.1d);
            }
            // frmTempSeuil.Label24.Text = CInt(Tseuil2 * 10) / 10
            form.temperature_seuil_epandage_automatique = (float)((int)Math.Round(Tseuil2 * 10f) / 10d);
            return form;
            // frmTempSeuil.ButtonExportFile.Show()
            // frmTempSeuil.ButtonExportDB.Show()
            // frmTempSeuil.LabelOR.Show()

        }

        private void CalNeige()
        {
            int i; // [-]
            float SeuilNeige; // °C

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // répartition de la neige
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            var k = default(int);
            float roh; // densité de neige [kg/m3]
            float cum = 0f;
            SeuilNeige = 4f;
            if (CasInput == 1) // avec données de neige moy80
            {
                var loopTo = iAnzahl - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    if (arrDaten[i].moy80 == 32767f | arrDaten[i].moy80 == 0f)
                    {
                        k = k + 1;
                        if (k > 15)
                        {
                            k = 15;
                        }
                    }
                    if (arrDaten[i].moy80 != 0f & arrDaten[i].moy80 != 32767f) // il neige
                    {
                        var loopTo1 = k - 1;
                        for (k = 0; k <= loopTo1; k++)
                        {
                            if (arrDaten[i - k].moy6 / 10f >= SeuilNeige)
                            {
                                arrDaten[i - k].neige = 0f;
                            }
                            else
                            {
                                if (arrDaten[i - k].moy6 / 10f <= -1)
                                {
                                    roh = 3f * arrDaten[i - k].moy6 / 10f + 110f;
                                }
                                else
                                {
                                    roh = 23f * arrDaten[i - k].moy6 / 10f + 130f;
                                }
                                arrDaten[i - k].neige = (float)(arrDaten[i - k].moy17 / 10d * 1000d / (double)roh);
                            }  // 1000 densité de l'eau=cte
                        }
                    }
                }
            }
            else // sans données de neige moy80
            {
                var loopTo2 = iAnzahl - 1;
                for (i = 0; i <= loopTo2; i++)
                {
                    if (arrDaten[i].moy6 / 10f >= SeuilNeige)
                    {
                        arrDaten[i].neige = 0f;
                    }
                    else
                    {
                        if (arrDaten[i].moy6 / 10f <= -1)
                        {
                            roh = 3f * arrDaten[i].moy6 / 10f + 110f;
                        }
                        else
                        {
                            roh = 23f * arrDaten[i].moy6 / 10f + 130f;
                        }
                        arrDaten[i].neige = (float)(arrDaten[i].moy17 / 10d * 1000d / (double)roh);
                    }    // 1000 densité de l'eau=cte
                    cum = cum + arrDaten[i].neige;
                    if (cum < 2f)
                    {
                        arrDaten[i].neige = 0f;
                    }
                    if (arrDaten[i].moy80 == 0f)
                    {
                        cum = 0f;
                    }
                }
            }
        }

        private void AttenBruit(ref float A, ref float B, ref float C, ref float D, ref float[] tempInput, ref float[] tempOutput, ref float tlim)
        {
            float dT1;
            float dT2;
            var bT1 = default(float);
            var bT2 = default(float);
            float T1;
            float T2;
            int i;
            int j;
            int k;
            var l = default(int);
            var PentePos = default(bool);

            var loopTo = iAnzahl - 1;
            for (i = 0; i <= loopTo; i++)
            {
                k = l;
                var loopTo1 = iAnzahl - 3;
                for (j = k; j <= loopTo1; j++) // trouve le min et le max température 
                {
                    dT1 = tempInput[j + 1] - tempInput[j];
                    dT2 = tempInput[j + 2] - tempInput[j + 1];
                    if (j == k)
                        bT1 = tempInput[j];
                    if (dT1 > 0f & j == k)
                    {
                        PentePos = true;
                    }
                    else if (dT1 < 0f & j == k)
                    {
                        PentePos = false;
                    }
                    else if (dT1 == 0f & j == k)
                    {
                        bT1 = tempInput[j + 1];
                    }
                    if (PentePos == true & dT2 < 0f)
                    {
                        bT2 = tempInput[j + 1];
                        break;
                    }
                    else if (PentePos == false & dT2 > 0f)
                    {
                        bT2 = tempInput[j + 1];
                        break;
                    }
                }
                bT1 = A * (bT2 - bT1) / B + bT1;  // calcul de la moyenne
                var loopTo2 = j;
                for (l = k; l <= loopTo2; l++)
                {
                    dT1 = tempInput[l + 1] - tempInput[l];
                    if (dT1 != 0f)
                    {
                        dT2 = C / (D * dT1);
                    }
                    else
                    {
                        dT2 = 0f;
                    }
                    if (Math.Abs(dT2) > tlim)
                        dT2 = 0f;
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
                    if (Math.Abs(bT1 - T2) < Math.Abs(bT1 - T1))
                    {
                        tempOutput[l] = T2;
                    }
                    else
                    {
                        tempOutput[l] = T1;
                    }
                }
                if (l == iAnzahl - 1)
                    break;
            }

        }


        public void WriteMeteoFormToTextFile(string outfile, StrctForm form, bool isCalcul)
        {
            // Ouvrir le fichier texte en mode écriture
            int nFic = FileSystem.FreeFile();
            var sw = new StreamWriter(outfile);


            sw.WriteLine(form.nb_annees);
            sw.WriteLine(form.concentration_chlorure);
            sw.WriteLine(form.epaisseur_film_eau_chaussee);
            sw.WriteLine(form.humidite_relative_seuil_intervention);

            sw.WriteLine(form.concentration_annuelle_chlorure_sodium_epandage_mecanique);
            sw.WriteLine(form.quantite_moyenne_chlorure_epandage_mecanique);
            if (isCalcul)
            {
                sw.WriteLine(form.nb_intervention_epandage);
            }
            // PrintLine(nFic, frmTempSeuil.Label6.Text)
            sw.WriteLine(form.intervalle_minimal_entre_2);
            sw.WriteLine(form.concentration_chlorure_sodium_epandage_mecanique);
            // PrintLine(nFic, frmTempSeuil.Label22.Text)
            if (isCalcul)
            {
                sw.WriteLine(form.temperature_seuil_epandage_mecanique);
            }
            sw.WriteLine(form.concentration_annuelle_chlorure_sodium_epandage_automatique);
            sw.WriteLine(form.quantite_moyenne_chlorure_epandage_automatique);
            if (isCalcul)
            {
                sw.WriteLine(form.nb_giclages_annuel);
            }
            // PrintLine(nFic, frmTempSeuil.Label76.Text)
            sw.WriteLine(form.nb_giclage_par_intervalle);
            sw.WriteLine(form.concentration_chlorure_sodium_epandage_automatique);
            // PrintLine(nFic, frmTempSeuil.Label66.Text)
            if (isCalcul)
            {
                sw.WriteLine(form.temperature_seuil_epandage_automatique);
            }
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

            // Fermer le fichier

        }

        public StrctForm ReadMeteoFormFromTextFile(string infile)
        {
            var form = new StrctForm();

            // Vérifier si le fichier existe
            if (!File.Exists(infile))
            {
                throw new FileNotFoundException("Le fichier spécifié n'existe pas.", infile);
            }

            // Lire le fichier ligne par ligne
            string[] lines = File.ReadAllLines(infile);
            int index = 0;

            try
            {
                form.nb_annees = (float)double.Parse(lines[index]);
                index += 1;
                form.concentration_chlorure = (float)double.Parse(lines[index]);
                index += 1;
                form.epaisseur_film_eau_chaussee = (float)double.Parse(lines[index]);
                index += 1;
                form.humidite_relative_seuil_intervention = (float)double.Parse(lines[index]);
                index += 1;

                form.concentration_annuelle_chlorure_sodium_epandage_mecanique = (float)double.Parse(lines[index]);
                index += 1;
                form.quantite_moyenne_chlorure_epandage_mecanique = (float)double.Parse(lines[index]);
                index += 1;

                form.nb_intervention_epandage = (float)double.Parse(lines[index]);
                index += 1;

                form.intervalle_minimal_entre_2 = (float)double.Parse(lines[index]);
                index += 1;
                form.concentration_chlorure_sodium_epandage_mecanique = (float)double.Parse(lines[index]);
                index += 1;

                form.temperature_seuil_epandage_mecanique = (float)double.Parse(lines[index]);
                index += 1;

                form.concentration_annuelle_chlorure_sodium_epandage_automatique = (float)double.Parse(lines[index]);
                index += 1;
                form.quantite_moyenne_chlorure_epandage_automatique = (float)double.Parse(lines[index]);
                index += 1;

                form.nb_giclages_annuel = (float)double.Parse(lines[index]);
                index += 1;

                form.nb_giclage_par_intervalle = (float)double.Parse(lines[index]);
                index += 1;
                form.concentration_chlorure_sodium_epandage_automatique = (float)double.Parse(lines[index]);
                index += 1;

                form.temperature_seuil_epandage_automatique = (float)double.Parse(lines[index]);
                index += 1;

                form.position_de_la_1_temperature_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.position_de_la_2_temperature_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_1_temperature_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_2_temperature_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.difference_de_temperature_exterieur = (float)double.Parse(lines[index]);
                index += 1;

                form.position_de_la_1_humidite_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.position_de_la_2_humidite_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_1_humidite_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_2_humidite_exterieur = (float)double.Parse(lines[index]);
                index += 1;
                form.difference_de_humidite_exterieur = (float)double.Parse(lines[index]);
                index += 1;

                form.position_de_la_1_temperature_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.position_de_la_2_temperature_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_1_temperature_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_2_temperature_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.difference_de_temperature_interieure = (float)double.Parse(lines[index]);
                index += 1;

                form.position_de_la_1_humidite_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.position_de_la_2_humidite_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_1_humidite_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.attenuation_de_2_humidite_interieure = (float)double.Parse(lines[index]);
                index += 1;
                form.difference_de_humidite_interieure = (float)double.Parse(lines[index]);
                index += 1;
            }

            catch (Exception ex)
            {
                throw new FormatException("Erreur lors de la lecture du fichier. Assurez-vous qu'il suit le bon format.", ex);
            }

            return form;
        }



    }
}