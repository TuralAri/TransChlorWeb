using System;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Net.Http.Json;
using Microsoft.VisualBasic.CompilerServices;

namespace TransChlorApi.Models
{

    public class Compute1D
    {
        //added by STAGIAIRE
        public int computationId { get; set; }
        public HttpClient httpClient { get; set; }

        public int nLayersExt {get; set;}
        public double[] epLayersExt { get; set; }        
        //END added by STAGIAIRE
        

        public int LgLim = 40;
        public double pPH = 12.6d;
        public double mPh = 6.5d;
        public int RoW = 1000;        // kg/m3
        public double R = 8.3145d;        // J/mol.K

        // public TransChlor.frmGraph1D frm;

        // Moisture Parameters 
        private float aa; // Function coefficient
        private float Hc; // Function coefficient
        private float ab; // Function coefficient
        private float tc; // Function coefficient
        private bool ImpHydr;
        // chloride
        private float[] LambdaT = new float[2];
        private float[] LambdaH = new float[2];
        private float aOH;
        private float EbG;
        private float toG;
        private float faG;
        // Moisture variables
        private float hMin;
        private float hEcart;
        private float wMin;
        private float wEcart;
        private float CTmin;
        private float CTecart;
        private float CLmin;
        private float CLecart;
        private float Tecart;
        private float H_snap;
        private float Retard;
        // Structural data
        private float TimeMax;
        private float Length; // length of the layer [mm]
        private short Ne; // Number of finite elements
        private decimal[] Le = new decimal[2]; // element length
        private decimal[] PosProf = new decimal[2];
        // Computational data
        private short Dofs;
        // computationals values
        private float DeltaT;
        // boundary conditions
        private short NEXPO;
        private short NQUAL;
        private float taff;
        private float Hsauv;
        private float Wsauv;
        private float CTsauv;
        private float CLsauv;
        private float Tsauv;
        private float Carbsauv;
        private string[] Filebeton = new string[2];
        private string[] Fileres = new string[2];
        private float[] PD = new float[2];
        private float[] qGran = new float[2];
        private float[] Dcl = new float[2];
        private float[] SAT = new float[2];
        private float[] ciment = new float[2];
        private string[] FileGexpo = new string[2];
        private string[] FileDexpo = new string[2];
        private float[,] proba = new float[2, 2];
        private short ChoixRep;
        private short nChmt;
        private short[] Nbreel = new short[2];
        private float[] LenApp = new float[2];
        private float[] EC = new float[2];
        private float[] EsC = new float[2];
        private float[] tProt = new float[2];
        private float[] Vct = new float[2];
        private float[] Nct = new float[2];
        private float capCal;
        private float[] Hydr = new float[2];
        private float[] ED = new float[2];
        private float[] ToHydr = new float[2];
        private float[] Ecl = new float[2];
        private float[] ToCl = new float[2];
        private double[] Ctherm = new double[2];
        private double[] Chydr = new double[2];
        private double[] Cion = new double[2];
        private string PostFile;
        private float GyCO2;
        private float DyCO2;
        private float[] RoA = new float[2];
        private float[] RoC = new float[2];

        private int PintermManual = 0;

        public void ReadFile(string OutFile, ref short Nbre1, ref short Nbre2, ref short Nbre3, ref double[,] Creadtherm, ref double[,] Creadhydr, ref double[,] Creadion)
        {

            // '''''''''''''''''''''''''''''''''''''''''''
            string Filtre = "Text files (INPUT_*.txt)|INPUT_*.txt";
            short Index = 1;
            bool Directoire = true;
            string Titre = "S�lectionner le fichier d'exposition";
            // var OutFile = default(string);
            // bool Canc = false;
            int nFic = FileSystem.FreeFile();
            short i, j;

            // TransChlor.modDialog.OpenDialog(ref OutFile, ref Canc, ref Filtre, ref Index, ref Directoire, ref Titre);
            // if (Canc == true)
            //     Environment.Exit(0);
            // '''''''''''''''''''''''''''''''''''''''''''

            FileSystem.FileOpen(nFic, OutFile, OpenMode.Input, OpenAccess.Read, OpenShare.Shared);
            // TransChlor.modDialog.FilePost(ref OutFile, ref PostFile);

            float Para1;
            float Para2;
            float Para3;
            var Para4 = default(float);
            float test;

            // FileSystem.Input(nFic, ref Length);
            // FileSystem.Input(nFic, ref Ne);
            // FileSystem.Input(nFic, ref ChoixRep);
            Length = float.Parse(FileSystem.LineInput(nFic));
            Ne = short.Parse(FileSystem.LineInput(nFic));
            ChoixRep = short.Parse(FileSystem.LineInput(nFic));
            
            Le = new decimal[Ne + 1 + 1];
            PosProf = new decimal[Ne + 2 + 1];

            PosProf[1] = 0m;
            switch (ChoixRep)
            {
                case 1:
                    {
                        var loopTo = Ne;
                        for (i = 1; i <= loopTo; i++)
                        {
                            Le[i] = (decimal)Length / Ne;
                            PosProf[i + 1] = PosProf[i] + Le[i];
                        }

                        break;
                    }

                case 2:
                    {
                        // FileSystem.Input(nFic, ref Le[1]);
                        Le[1] = decimal.Parse(FileSystem.LineInput(nFic));
                        Para1 = 0f;
                        Para2 = 0f;
                        var loopTo1 = (short)Math.Round(Ne / 2d - 1d);
                        for (i = 1; i <= loopTo1; i++)
                        {
                            Para1 = Para1 + 1f;
                            Para2 = Para2 + Para1;
                        }
                        if (Le[1] > (decimal)Length)
                            Environment.Exit(0);
                        Para1 = (Length / 2f - Ne / 2f * (float)Le[1]) / Para2;
                        PosProf[2] = Le[1];
                        PosProf[Ne + 1] = (decimal)Length;
                        PosProf[Ne] = (decimal)Length - Le[1];
                        var loopTo2 = (short)Math.Round(Ne / 2d);
                        for (i = 2; i <= loopTo2; i++)
                        {
                            Le[i] = Le[1] + (i - 1m) * (decimal)Para1;
                            PosProf[i + 1] = PosProf[i] + Le[i];
                            Le[Ne - i] = Le[i];
                            PosProf[(short)(Ne + 1) - i] = PosProf[(short)(Ne + 2) - i] - Le[i];
                        }

                        break;
                    }
                case 3:
                    {
                        // FileSystem.Input(nFic, ref Le[1]);
                        Le[1] = decimal.Parse(FileSystem.LineInput(nFic));
                        Para3 = (float)Le[1];
                        test = 10f;
                        if (Le[1] > (decimal)Length)
                            Environment.Exit(0);
                        while ((double)Math.Abs(test) > 0.0001d)
                        {
                            Para1 = 1f - Length / (2f * (float)Le[1]);
                            Para2 = 1f;
                            var loopTo3 = (short)Math.Round(Ne / 2d - 1d);
                            for (i = 1; i <= loopTo3; i++)
                            {
                                Para1 += (float)Math.Pow((double)Para3, i);
                                if (i < Ne / (double)(short)2 - 1)
                                    Para2 += (float)((i + 1) * Math.Pow((double)Para3, i));
                            }
                            Para4 = Para3 - Para1 / Para2;
                            test = Para4 - Para3;
                            Para3 = Para4;
                        }
                        PosProf[2] = Le[1];
                        PosProf[Ne + 1] = (decimal)Length;
                        PosProf[Ne] = (decimal)Length - Le[1];
                        var loopTo4 = (short)Math.Round(Ne / 2d);
                        for (i = 2; i <= loopTo4; i++)
                        {
                            Le[i] = (decimal)((double)Le[1] * Math.Pow((double)Para4, i - 1));
                            PosProf[i + 1] = PosProf[i] + Le[i];
                            Le[Ne - i] = Le[i];
                            PosProf[(short)(Ne + 1) - i] = PosProf[(short)(Ne + 2) - i] - Le[i];
                        }

                        break;
                    }
                case 4:
                    {
                        // FileSystem.Input(nFic, ref nChmt);
                        nChmt = short.Parse(FileSystem.LineInput(nFic));
                        Nbreel = new short[nChmt - 1 + 1];
                        LenApp = new float[nChmt - 1 + 1];
                        PosProf[Ne + 1] = (decimal)Length;
                        Para1 = 0f;
                        Para2 = 0f;
                        var loopTo5 = (short)(nChmt - 1);
                        for (i = 1; i <= loopTo5; i++)
                        {
                            // FileSystem.Input(nFic, ref Nbreel[i]);
                            Nbreel[i] = short.Parse(FileSystem.LineInput(nFic));
                            // FileSystem.Input(nFic, ref LenApp[i]);
                            LenApp[i] = float.Parse(FileSystem.LineInput(nFic));
                            var loopTo6 = Nbreel[i];
                            for (j = 1; j <= loopTo6; j++)
                            {
                                Le[j + (short)Math.Round(Para1)] = (decimal)LenApp[i] / Nbreel[i];
                                PosProf[(short)(j + 1) + (short)Math.Round(Para1)] = PosProf[j + (short)Math.Round(Para1)] + Le[j + (short)Math.Round(Para1)];
                                Le[(short)((short)(Ne + 1) - j) - (short)Math.Round(Para1)] = Le[j + (short)Math.Round(Para1)];
                                PosProf[(short)((short)(Ne + 1) - j) - (short)Math.Round(Para1)] = PosProf[(short)((short)(Ne + 2) - j) - (short)Math.Round(Para1)] - Le[j + (short)Math.Round(Para1)];
                            }
                            Para1 = Para1 + Nbreel[i];
                            Para2 = Para2 + LenApp[i];
                        }
                        Para3 = Ne - 2f * Para1;
                        Para2 = Length - 2f * Para2;
                        var loopTo7 = (short)((short)Math.Round(Para1) + (short)Math.Round(Para3));
                        for (i = (short)((short)Math.Round(Para1) + 1); i <= loopTo7; i++)
                        {
                            Le[i] = (decimal)Para2 / (decimal)Para3;
                            PosProf[i + 1] = PosProf[i] + Le[i];
                        }

                        break;
                    }
                case 5:
                    {
                        // FileSystem.Input(nFic, ref nChmt);
                        nChmt = short.Parse(FileSystem.LineInput(nFic));
                        Nbreel = new short[nChmt - 1 + 1];
                        LenApp = new float[nChmt - 1 + 1];
                        PosProf[Ne + 1] = (decimal)Length;
                        Para1 = 0f;
                        Para2 = 0f;
                        var loopTo8 = (short)(nChmt - 1);
                        for (i = 1; i <= loopTo8; i++)
                        {
                            // FileSystem.Input(nFic, ref Nbreel[i]);
                            // FileSystem.Input(nFic, ref LenApp[i]);
                            Nbreel[i] = short.Parse(FileSystem.LineInput(nFic));
                            LenApp[i] = float.Parse(FileSystem.LineInput(nFic));
                            var loopTo9 = Nbreel[i];
                            for (j = 1; j <= loopTo9; j++)
                            {
                                Le[j + (short)Math.Round(Para1)] = (decimal)LenApp[i] / Nbreel[i];
                                PosProf[(short)(j + 1) + (short)Math.Round(Para1)] = PosProf[j + (short)Math.Round(Para1)] + Le[j + (short)Math.Round(Para1)];
                            }
                            Para1 = Para1 + Nbreel[i];
                            Para2 = Para2 + LenApp[i];
                        }
                        Para3 = Ne - Para1;
                        Para2 = Length - Para2;
                        var loopTo10 = (short)((short)Math.Round(Para1) + (short)Math.Round(Para3));
                        for (i = (short)((short)Math.Round(Para1) + 1); i <= loopTo10; i++)
                        {
                            Le[i] = (decimal)Para2 / (decimal)Para3;
                            PosProf[i + 1] = PosProf[i] + Le[i];
                        }

                        break;
                    }
            }

            TimeMax = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref TimeMax);
            DeltaT = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref DeltaT);
            taff = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref taff);
            Hsauv = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Hsauv);
            Wsauv = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Wsauv);
            CTsauv = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref CTsauv);
            CLsauv = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref CLsauv);
            Tsauv = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Tsauv);
            Carbsauv = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Carbsauv);
            hMin = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref hMin);
            hEcart = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref hEcart);
            wMin = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref wMin);
            wEcart = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref wEcart);
            CLmin = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref CLmin);
            CLecart = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref CLecart);
            CTmin = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref CTmin);
            CTecart = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref CTecart);
            Tecart = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Tecart);
            aa = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref aa);
            Hc = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Hc);
            ab = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref ab);
            tc = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref tc);
            ImpHydr = bool.Parse(FileSystem.LineInput(nFic));
            // FileSystem.Input(nFic, ref ImpHydr);
            H_snap = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref H_snap);
            Retard = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref Retard);
            aOH = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref aOH);
            EbG = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref EbG);
            toG = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref toG);
            faG = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref faG);
            capCal = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref capCal);
            GyCO2 = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref GyCO2);
            DyCO2 = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref DyCO2);

            NEXPO = short.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            // FileSystem.Input(nFic, ref NEXPO);
            FileGexpo = new string[(NEXPO + 1)];
            FileDexpo = new string[(NEXPO + 1)];
            var loopTo11 = NEXPO;
            for (i = 1; i <= loopTo11; i++)
            {
                FileGexpo[i] = FileSystem.LineInput(nFic);
                // FileSystem.Input(nFic, ref FileGexpo[i]);
                FileDexpo[i] = FileSystem.LineInput(nFic);
                // FileSystem.Input(nFic, ref FileDexpo[i]);
            }

            // FileSystem.Input(nFic, ref NQUAL);
            NQUAL = short.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            Filebeton = new string[(NQUAL + 1)];
            Fileres = new string[(NQUAL + 1)];
            PD = new float[(NQUAL + 1)];
            Dcl = new float[(NQUAL + 1)];
            qGran = new float[(NQUAL + 1)];
            LambdaH = new float[(NQUAL + 1)];
            LambdaT = new float[(NQUAL + 1)];
            SAT = new float[(NQUAL + 1)];
            ciment = new float[(NQUAL + 1)];
            EC = new float[(NQUAL + 1)];
            tProt = new float[(NQUAL + 1)];
            Vct = new float[(NQUAL + 1)];
            Nct = new float[(NQUAL + 1)];
            Hydr = new float[(NQUAL + 1)];
            ED = new float[(NQUAL + 1)];
            ToHydr = new float[(NQUAL + 1)];
            Ecl = new float[(NQUAL + 1)];
            ToCl = new float[(NQUAL + 1)];
            RoA = new float[(NQUAL + 1)];
            RoC = new float[(NQUAL + 1)];
            proba = new float[(NQUAL + 1), 20];
            EsC = new float[(NQUAL + 1)];
            var loopTo12 = NQUAL;
            for (i = 1; i <= loopTo12; i++)
            {
                Filebeton[i] = FileSystem.LineInput(nFic);
                // FileSystem.Input(nFic, ref Filebeton[i]);
                Fileres[i] = FileSystem.LineInput(nFic);
                // FileSystem.Input(nFic, ref Fileres[i]);
                PD[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref PD[i]);
                Dcl[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Dcl[i]);
                qGran[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref qGran[i]);
                LambdaH[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref LambdaH[i]);
                LambdaT[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref LambdaT[i]);
                SAT[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref SAT[i]);
                ciment[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref ciment[i]);
                EC[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref EC[i]);
                tProt[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref tProt[i]);
                Hydr[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Hydr[i]);
                Vct[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Vct[i]);
                Nct[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Nct[i]);
                ED[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref ED[i]);
                ToHydr[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref ToHydr[i]);
                Ecl[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Ecl[i]);
                ToCl[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref ToCl[i]);
                RoA[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref RoA[i]);
                RoC[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref RoC[i]);
                for (j = 0; j <= 19; j++)
                {
                    proba[i,j] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                    // FileSystem.Input(nFic, ref proba[i, j]);
                }
                    
                EsC[i] = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref EsC[i]);
            }

            Creadtherm = new double[2, 2];
            Creadhydr = new double[2, 2];
            Creadion = new double[2, 2];

            // FileSystem.Input(nFic, ref Nbre1);
            Nbre1 = short.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            Creadtherm = new double[2, (Nbre1 + 1)];
            var loopTo13 = Nbre1;
            for (i = 1; i <= loopTo13; i++)
            {
                Creadtherm[0,i] = double.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Creadtherm[0, i]);
                Creadtherm[1,i] = double.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Creadtherm[1, i]);
            }

            // FileSystem.Input(nFic, ref Nbre2);
            Nbre2 = short.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            Creadhydr = new double[2, (Nbre2 + 1)];
            var loopTo14 = Nbre2;
            for (i = 1; i <= loopTo14; i++)
            {
                Creadhydr[0,i] = double.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Creadhydr[0, i]);
                Creadhydr[1,i] = double.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Creadhydr[1, i]);
                Creadhydr[1, i] = Creadhydr[1, i] / 100d;
            }

            // FileSystem.Input(nFic, ref Nbre3);
            Nbre3 = short.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
            Creadion = new double[2, (Nbre3 + 1)];
            var loopTo15 = Nbre3;
            for (i = 1; i <= loopTo15; i++)
            {
                Creadion[0,i] = double.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Creadion[0, i]);
                Creadion[1,i] = double.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                // FileSystem.Input(nFic, ref Creadion[1, i]);
            }

            try
            {
                PintermManual = int.Parse(FileSystem.LineInput(nFic));
                // FileSystem.Input(nFic, ref PintermManual);
                // Interaction.MsgBox("Le nombre de points est de" + PintermManual.ToString(), MsgBoxStyle.OkOnly & MsgBoxStyle.Information, "Information Points de discretisation");
                Console.WriteLine($"Le nombre de points est de\" + {PintermManual.ToString()}");
            }
            catch
            {
                // Interaction.MsgBox("Attention aucun points pris en compte manuellement", MsgBoxStyle.OkOnly & MsgBoxStyle.Information, "Information Points de discretisation");
                Console.WriteLine("Attention aucun points pris en compte manuellement");
            }

            FileSystem.FileClose(nFic);

        }

        public void InitialConditions(ref short Nbre1, ref short Nbre2, ref short Nbre3, ref double[,] Creadtherm, ref double[,] Creadhydr, ref double[,] Creadion)
        {

            Dofs = (short)(Ne + 1);

            // calcul des conditions initiales
            short i;
            short j = 0;
            Ctherm = new double[Dofs + 1 + 1];
            double Var, Var1;

            var loopTo = (short)(Nbre1 - 1);
            for (i = 1; i <= loopTo; i++)
            {
                Var = (Creadtherm[1, i + 1] - Creadtherm[1, i]) / (Creadtherm[0, i + 1] - Creadtherm[0, i]);
                Var1 = Creadtherm[1, i] - Var * Creadtherm[0, i];
                if (j <= Dofs)
                {
                    while ((double)PosProf[j] <= Creadtherm[0, i + 1])
                    {
                        Ctherm[j] = Var * (double)PosProf[j] + Var1;
                        j = (short)(j + 1);
                        if (j > Dofs)
                            break;
                    }
                }
                Ctherm[Dofs + 1] = Ctherm[Dofs];
            }
            j = 0;

            Chydr = new double[Dofs + 1 + 1];
            var loopTo1 = (short)(Nbre2 - 1);
            for (i = 1; i <= loopTo1; i++)
            {
                Var = (Creadhydr[1, i + 1] - Creadhydr[1, i]) / (Creadhydr[0, i + 1] - Creadhydr[0, i]);
                Var1 = Creadhydr[1, i] - Var * Creadhydr[0, i];
                if (j <= Dofs)
                {
                    while ((double)PosProf[j] <= Creadhydr[0, i + 1])
                    {
                        Chydr[j] = Var * (double)PosProf[j] + Var1;
                        j = (short)(j + 1);
                        if (j > Dofs)
                            break;
                    }
                }
                Chydr[Dofs + 1] = Chydr[Dofs];
                if (j > Dofs + 1)
                    break;
            }
            j = 0;

            Cion = new double[Dofs + 1 + 1];
            var loopTo2 = (short)(Nbre3 - 1);
            for (i = 1; i <= loopTo2; i++)
            {
                Var = (Creadion[1, i + 1] - Creadion[1, i]) / (Creadion[0, i + 1] - Creadion[0, i]);
                Var1 = Creadion[1, i] - Var * Creadion[0, i];
                if (j <= Dofs)
                {
                    while ((double)PosProf[j] <= Creadion[0, i + 1])
                    {
                        Cion[j] = Var * (double)PosProf[j] + Var1;
                        j = (short)(j + 1);
                        if (j > Dofs)
                            break;
                    }
                }
                Cion[Dofs + 1] = Cion[Dofs];
            }

            Le[0] = 1m;       // couche limite
            Le[Dofs] = 1m;    // couche limite

        }

        public void InitFrmGraph()
        {

            // frm = new TransChlor.frmGraph1D();
            // frm.MdiParent = TransChlor.My.MyProject.Forms.MDIChlor;
            // frm.Left = 0;
            // frm.Top = 0;
            // frm.Height = TransChlor.My.MyProject.Forms.MDIChlor.Height;
            // frm.Width = TransChlor.My.MyProject.Forms.MDIChlor.Width;

            // frm.Show();

        }

        public bool ReadExpo(ref string INFile, ref int NbreEn, ref float Fit, ref decimal[] Temperature, ref float[] Humidite, ref float[] Sel, ref decimal Msel, ref float[] SAT, ref decimal TempMin, ref float TempMax, ref float TempEcart, ref int nCouches, ref int NQUAL, ref bool bordG)
        {

            short nFic = (short)FileSystem.FreeFile();
            var wsat = default(float);

            if (INFile.Contains(".txt") == true)
            {

                try
                {
                    Console.WriteLine("on ouvre le fichier");
                    Console.WriteLine($"{INFile},{nFic}");
                    FileSystem.FileOpen(nFic, INFile, OpenMode.Input, OpenAccess.Read);
                    Console.WriteLine("on a bien ouvert");
                    // FileSystem.Input(nFic, ref NbreEn);
                    // FileSystem.Input(nFic, ref Fit);
                    NbreEn = int.Parse(FileSystem.LineInput(nFic));
                    Console.WriteLine("OK 1");
                    Fit = float.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                    Humidite = new float[NbreEn + 1];
                    Sel = new float[NbreEn + 1];
                    Temperature = new decimal[NbreEn + 1];
                    if (nCouches > 1) // 03.10.2023 d�but
                    {
                        if (bordG == true)
                        {
                            wsat = SAT[1];
                        }
                        else
                        {
                            wsat = SAT[nCouches];
                        }
                    }  // 03.10.2023 fin

                    for (int j = 1, loopTo = NbreEn; j <= loopTo; j++)
                    {
                        FileSystem.Input(nFic, ref Humidite[j]);
                        FileSystem.Input(nFic, ref Sel[j]);
                        // FileSystem.Input(nFic, ref Temperature[j]);
                        Temperature[j] = decimal.Parse(FileSystem.LineInput(nFic), CultureInfo.InvariantCulture);
                        if ((float)Temperature[j] > TempMax)
                            TempMax = (float)Temperature[j];
                        if (Temperature[j] < TempMin)
                            TempMin = Temperature[j];
                        Sel[j] *= (float)(35.453d / 58.443d);    // calcul de cT � partir de co � multiplier par w(0) ou (dofs)
                        if ((float)Msel < Sel[j] * wsat)
                            Msel = (decimal)(Sel[j] * wsat);
                    }
                    FileSystem.FileClose(nFic);
                }

                catch (Exception ex)
                {
                    return false;
                }
            }

            else
            {

                try
                {

                    // var DBCon = new TransChlor.DBconnexion();
                    // Dim Expo As New MaterialsData

                    // string argRequest = "SELECT * FROM [" + INFile + "]";
                    // DBCon.DBRequest(ref argRequest);
                    // DBCon.MatFill(Expo, INFile)

                    // Dim ExpoTable()() As Object = Expo.Tables(INFile).Rows.Cast(Of DataRow).Select(Function(dr) dr.ItemArray).ToArray

                    // NbreEn = ExpoTable.Count()
                    Fit = 3600f;

                    Humidite = new float[NbreEn + 1];
                    Sel = new float[NbreEn + 1];
                    Temperature = new decimal[NbreEn + 1];

                    for (int j = 0, loopTo1 = NbreEn - 1; j <= loopTo1; j++)
                    {
                        // Humidite(j) = CSng(ExpoTable(j)(1))
                        // Sel(j) = CSng(ExpoTable(j)(2))
                        // Temperature(j) = CDec(ExpoTable(j)(3))
                        if ((float)Temperature[j] > TempMax)
                            TempMax = (float)Temperature[j];
                        if (Temperature[j] < TempMin)
                            TempMin = Temperature[j];
                        Sel[j] *= (float)(35.453d / 58.443d); // calcul de cT � partir de co � multiplier par w(0) ou (dofs)
                        if ((float)Msel < Sel[j] * wsat)
                            Msel = (decimal)(Sel[j] * wsat);
                    }
                }

                catch (Exception ex)
                {
                    return false;
                }

            }

            return true;

        }

        // Coeur du calcul
        public void Compute_All(CancellationToken cancellationToken)
        {

            // Moisture variables
            var H_old = new decimal[2]; // Moisture potential at beginning interval[-]  (p/ps)
            var H_new = new decimal[2]; // Moisture potential at end of interval
            var H_trial = new decimal[2]; // Trial moisture potential during iteration
            var Hold = new decimal[2]; // H limit on desorption line [-]
            var W = new decimal[2]; // water content [kg/m3]
            var T_old = new decimal[2];
            var T_new = new decimal[2];
            var T_trial = new decimal[2];
            var C_old = new decimal[2];
            var C_new = new decimal[2];
            var C_trial = new decimal[2];
            var CBo = new decimal[2];
            decimal GCboundary;
            var DCboundary = default(decimal);
            var CT = new decimal[2];
            var LHS = new decimal[3, 2];
            var RHS = new decimal[2];
            var Speed = new decimal[2];

            long i;
            long i_day;
            long iG;
            long iD;
            long j;
            short jj;
            short jjj;
            long k;
            short Boucle1;
            short Boucle2;
            short Bou2;
            short Boucle3;
            short Boucle4;
            short Boucle5;
            short Boucle6;
            decimal Ha;
            decimal Mcap;
            var f1 = default(decimal);
            decimal f2;
            var f3 = default(decimal);
            decimal f5;
            decimal C11;
            decimal C12;
            decimal test;
            var testOld = default(decimal);
            bool ExB = false;
            decimal Position;
            var Cond01 = default(short);
            double Hteller;
            double Wteller;
            double CTteller;
            double CLteller;
            double Tteller;
            double Carbteller;
            double affiche;
            var GTemperature = new decimal[2];
            var GHumidite = new float[2];
            var GSel = new float[2];
            var DTemperature = new decimal[2];
            var DHumidite = new float[2];
            var DSel = new float[2];
            string GINFile;
            string DINFile;
            var OutFile = new string[8];
            float DelT;
            float FiT;
            var GFiT = default(float);
            var DFiT = default(float);
            long NbreEn;
            var GNbreEn = default(long);
            var DNbreEn = default(long);
            short Dim1;
            short Dim2;
            float t1;
            float t2;
            short nFic;
            short nFic1;
            short nFic2;
            short nFic3;
            short nFic4;
            short nFic5;
            short nFic6;
            short nFic7;

            // Dim nFic8 As Short      'prov
            float XdHmax;
            float XdCTmax;
            float XdTmax;
            long Tsec;
            short Num;
            string GSTa;
            string DSTa;
            var Pinterm = default(short);
            var Interm = new float[9];
            float dHmax;
            float dTmax;
            float dCTmax;
            var GdelTemp = default(float);
            var GdelH = default(float);
            var GdelCF = default(float);
            var DdelTemp = default(float);
            var DdelH = default(float);
            var DdelCF = default(float);
            float GTextold;
            float GHextold;
            float GCFextold;
            float GCTextold;
            float DTextold;
            float DHextold;
            float DCFextold;
            float DCTextold;
            var GTextern = default(float);
            var GHextern = default(float);
            var GCFextern = default(float);
            float GCTextern;
            var DTextern = default(float);
            var DHextern = default(float);
            var DCFextern = default(float);
            float DCTextern;
            decimal Tijd;
            var tijdOld = default(decimal);
            float testing1;
            float testing2;
            float testing3;
            // Dim Compteur(5) As Short
            float TempMin = 40f;
            float TempMax = -30;
            float TempEcart = 0f;
            short NprobHr;
            short NprobCap;
            short NprobCl;
            short NprobCa;
            double PdTot;
            var PdHr = new double[3];
            var PdCap = new double[3];
            var PdCl = new double[3];
            var PdCa = new double[3];
            var CoefHr = new double[3];
            var CoefCap = new double[3];
            var CoefCl = new double[3];
            var CoefCa = new double[3];
            string FileProb;
            var FprobHr = new string[3];
            var FprobCap = new string[3];
            var FprobCl = new string[3];
            var FprobCa = new string[3];
            // Dim Di As Single ' diffusion suppression 3.10.2023
            decimal CumW; // water content cumul� [kg/m3]
            decimal CumH; // humidit� relative moyen
                          // Dim CumClT As Decimal ' total chloride ions cumul� [kg/m3]
            float Wsat; // Saturated moisture content [kg/m3]   suppression 3.10.2023
            bool BDlibre;
            long Gcptj;
            long Dcptj;
            long Gctj;
            long Dctj;
            short Nbn = 0;
            decimal Ltot = 0m;
            var HAncien = new decimal[Dofs + 1 + 1];
            var W_old = new decimal[Dofs + 1 + 1];
            var Ae = new decimal[Dofs + 1 + 1];
            var Be = new decimal[Dofs + 1 + 1];
            decimal ClNeg = 0m;
            decimal c_int;
            var Ph = new decimal[(Dofs + 1)];
            var Gamma = new float[Dofs + 1 + 1];
            bool TW;
            float tPrec = 0f;
            bool Wol = false;
            float DRHe;
            float GRHe;
            float CumTemps;
            long iGco = 1L;
            long iDco = 1L;
            var Gxc = default(double);
            var Dxc = default(double);
            double Gxcold;
            double Dxcold;
            float GPH;
            float DPH;
            decimal Msel = 0m;
            short CB = 1;
            short DB = 1;
            short jEnd = 0;
            short jStart = 0;
            float bSup = 0f;
            float bInf = 0f;
            bool SingVal = false;
            float PenteA = 0f;
            float valB = 0f;
            decimal parA;     // param�tre pour le calcul du coefficient de capillarit�
            decimal parB;
            decimal parA1;
            decimal parB1;
            decimal parC1;
            decimal Hleft;
            decimal Hright;
            decimal Tleft;
            decimal Tright;
            var DcapLeft = default(decimal);
            var DcapRight = default(decimal);
            bool ptest;
            decimal Ctest;
            double CTmax;
            double CLmax;
            var CTmold = default(int);
            var CLmold = default(int);
            var Hsym = default(bool);
            var Ssym = default(bool);
            var Tsym = default(bool);
            int nCouches;
            var EpCouches = new double[2];
            var Can = default(short);

            // redimension variables
            T_new = new decimal[Dofs + 1 + 1];
            T_old = new decimal[Dofs + 1 + 1];
            T_trial = new decimal[Dofs + 1 + 1];
            H_new = new decimal[Dofs + 1 + 1];
            H_old = new decimal[Dofs + 1 + 1];
            H_trial = new decimal[Dofs + 1 + 1];
            Hold = new decimal[Dofs + 1 + 1];
            W = new decimal[Dofs + 1 + 1];
            LHS = new decimal[3, Dofs + 1 + 1];
            RHS = new decimal[Dofs + 1 + 1];
            C_old = new decimal[Dofs + 1 + 1];
            C_new = new decimal[Dofs + 1 + 1];
            C_trial = new decimal[Dofs + 1 + 1];
            CBo = new decimal[Dofs + 1 + 1];
            CT = new decimal[Dofs + 1 + 1];
            Speed = new decimal[Dofs + 1 + 1];

            // version sept 15th 2002
            // First initialize all variables

            // TransChlor.My.MyProject.Forms.MDIChlor.Prefile = PostFile;

            Hteller = Hsauv;
            Wteller = Wsauv;
            CTteller = CTsauv;
            CLteller = CLsauv;
            Tteller = Tsauv;
            Carbteller = Carbsauv;
            affiche = taff;

            dHmax = 0.1f; // convergence conditions limites (humidit� relative)
            testing1 = 0.0001f; // convergence it�ration (humidit� relative)
            dCTmax = 0.1f * 8f; // convergence conditions limites (ions chlorures)
            testing2 = 0.000025f * 8f; // convergence it�ration (ions chlorures)
            XdHmax = 0.2f; // grand saut du aux CL
            XdCTmax = 8f / 3f; // grand saut du aux CL
            DelT = DeltaT;
            TempMin = -10;
            TempMax = 40f;
            // For j = CShort(1) To CShort(5)
            // Compteur(j) = CShort(0)
            // Next j
            // programmation pour obtenir des fichiers de comparaison (provisoire)
            // Dim nFile As Integer = FreeFile()
            // Dim HRCUMUL(1000, 20) As Decimal
            // Dim cpt1 As Integer
            // Dim cpt2 As Integer
            // FileOpen(CInt(nFile), "R_HR_cumul.txt", OpenMode.Output)

            var loopTo = NEXPO;
            for (Boucle1 = 1; Boucle1 <= loopTo; Boucle1++) // exposition direct, �claboussures, brouillard
            {
                // if (NQUAL > 1)
                //     Can = (short)Interaction.MsgBox("Y a-t-il plusieurs couches ?", MsgBoxStyle.YesNo, "Avertissment"); // 03.10.2023 d�but
                // if (Can == (int)MsgBoxResult.Yes)
                if(nLayersExt > 1)
                {
                    nCouches = NQUAL;
                }
                else
                {
                    nCouches = 1;
                }
                if (nCouches > 1)
                {
                    EpCouches = new double[nCouches];
                    NQUAL = 1;
                    EpCouches[0] = 0d;
                    var loopTo1 = (long)(nCouches - 1);
                    for (i = 1L; i <= loopTo1; i++)
                        // EpCouches[(int)i] = Conversions.ToDouble(Interaction.InputBox("�paisseur de la couche no" + i + "? (max " + Length + " mm)", "Question", Length.ToString())) + EpCouches[(int)(i - 1L)];
                        EpCouches[i] = epLayersExt[i - 1] + EpCouches[i - 1];
                }          // 03.10.2023 fin
                var loopTo2 = NQUAL;
                for (Boucle2 = 1; Boucle2 <= loopTo2; Boucle2++) // qualit� du b�ton bonne, moyenne, mauvaise
                {
                    Bou2 = Boucle2;
                    if (proba[Boucle2, 0] == 0f)
                    {
                        NprobHr = 1;
                        PdHr[1] = 1d;
                        CoefHr[1] = 1d;
                        FprobHr[1] = "0";
                    }
                    else
                    {
                        NprobHr = 2;
                        CoefHr[1] = proba[Boucle2, 1];
                        CoefHr[2] = proba[Boucle2, 2];
                        PdHr[1] = proba[Boucle2, 3];
                        PdHr[2] = proba[Boucle2, 4];
                        FprobHr[1] = "1";
                        FprobHr[2] = "2";
                    }
                    if (proba[Boucle2, 5] == 0f)
                    {
                        NprobCap = 1;
                        PdCap[1] = 1d;
                        CoefCap[1] = 1d;
                        FprobCap[1] = "0";
                    }
                    else
                    {
                        NprobCap = 2;
                        CoefCap[1] = proba[Boucle2, 6];
                        CoefCap[2] = proba[Boucle2, 7];
                        PdCap[1] = proba[Boucle2, 8];
                        PdCap[2] = proba[Boucle2, 9];
                        FprobCap[1] = "1";
                        FprobCap[2] = "2";
                    }
                    if (proba[Boucle2, 10] == 0f)
                    {
                        NprobCl = 1;
                        PdCl[1] = 1d;
                        CoefCl[1] = 1d;
                        FprobCl[1] = "0";
                    }
                    else
                    {
                        NprobCl = 2;
                        CoefCl[1] = proba[Boucle2, 11];
                        CoefCl[2] = proba[Boucle2, 12];
                        PdCl[1] = proba[Boucle2, 13];
                        PdCl[2] = proba[Boucle2, 14];
                        FprobCl[1] = "1";
                        FprobCl[2] = "2";
                    }
                    if (proba[Boucle2, 15] == 0f)
                    {
                        NprobCa = 1;
                        PdCa[1] = 1d;
                        CoefCa[1] = 1d;
                        FprobCa[1] = "0";
                    }
                    else
                    {
                        NprobCa = 2;
                        CoefCa[1] = proba[Boucle2, 16];
                        CoefCa[2] = proba[Boucle2, 17];
                        PdCa[1] = proba[Boucle2, 18];
                        PdCa[2] = proba[Boucle2, 19];
                        FprobCa[1] = "1";
                        FprobCa[2] = "2";
                    }
                    var loopTo3 = NprobHr;
                    for (Boucle3 = 1; Boucle3 <= loopTo3; Boucle3++) // approche probabiliste sur humidit� relative
                    {
                        var loopTo4 = NprobCap;
                        for (Boucle4 = 1; Boucle4 <= loopTo4; Boucle4++) // approche probabiliste sur l'eau liquide
                        {
                            var loopTo5 = NprobCl;
                            for (Boucle5 = 1; Boucle5 <= loopTo5; Boucle5++) // approche probabiliste sur humidit� relative
                            {
                                var loopTo6 = NprobCa;
                                for (Boucle6 = 1; Boucle6 <= loopTo6; Boucle6++) // approche probabiliste sur humidit� relative
                                {
                                    PdTot = PdHr[Boucle3] * PdCap[Boucle4] * PdCl[Boucle5] * PdCa[Boucle6];
                                    FileProb = FprobHr[Boucle3] + FprobCap[Boucle4] + FprobCl[Boucle5] + FprobCa[Boucle6];
                                    // cpt1 = cpt1 + 1
                                    // cpt2 = 0
                                    CB = 1;
                                    DB = 1;
                                    iG = 1L;
                                    iD = 1L;
                                    iGco = 1L;
                                    iDco = 1L;
                                    tPrec = 0f;
                                    DRHe = 0f;
                                    GRHe = 0f;
                                    Gxcold = 0d;
                                    Dxcold = 0d;
                                    CumTemps = 0f;
                                    Msel = 0m;
                                    BDlibre = false;
                                    CLmax = 1d;
                                    CTmax = 1d;

                                    // coefficient de diffusion des ions de cl- + teneur en eau satur�
                                    // Di = Dcl(Boucle2) 'mm2/s 3.10.2023 suppression
                                    Wsat = SAT[Boucle2]; // kg/m3 bon b�ton 
                                    if (nCouches > 1) // 03.10.2023 d�but
                                    {
                                        var loopTo7 = (long)nCouches;
                                        for (j = 1L; j <= loopTo7; j++)
                                        {
                                            if (Wsat < SAT[(int)j])
                                                Wsat = SAT[(int)j];
                                        }
                                    }     // 03.10.2023 fin
                                    Tijd = 0m;
                                    var loopTo8 = (long)Dofs;
                                    for (i = 0L; i <= loopTo8; i++)  // conditions aux limites sur les noeuds
                                    {
                                        // Cion(i) = Cion(i) * Wsat * 35.453 / 58.443
                                        H_old[(int)i] = (decimal)Chydr[(int)i];
                                        H_new[(int)i] = (decimal)Chydr[(int)i];
                                        H_trial[(int)i] = (decimal)Chydr[(int)i];
                                        Hold[(int)i] = (decimal)Chydr[(int)i];
                                        HAncien[(int)i] = (decimal)Chydr[(int)i];
                                        T_old[(int)i] = (decimal)Ctherm[(int)i];
                                        T_new[(int)i] = (decimal)Ctherm[(int)i];
                                        T_trial[(int)i] = (decimal)Ctherm[(int)i];
                                        if (nCouches > 1) // 03.10.2023 d�but
                                        {
                                            var loopTo9 = (long)(nCouches - 1);
                                            for (j = 1L; j <= loopTo9; j++)
                                            {
                                                if ((double)PosProf[(int)i] < EpCouches[(int)j])
                                                {
                                                    W[(int)i] = TransChlorApi.Models.Functions.Water((decimal)Chydr[(int)i], HAncien[(int)i], ref T_new[(int)i], ref Tijd, ref tProt[(int)j], ref Vct[(int)j], ref Nct[(int)j], ref EC[(int)j], ref SAT[(int)j], ref Hydr[(int)j], ref ciment[(int)j], ref Wol);
                                                    Ph[(int)i] = (decimal)pPH;
                                                    Gamma[(int)i] = (float)(Math.Exp(aOH * (1d - Math.Pow(10d, (double)Ph[(int)i] - pPH))) * Math.Exp(EbG / R * (1d / (273.16d + (double)T_new[(int)i]) - (double)(1f / toG))) * ciment[(int)j] * Hydr[(int)j] * faG / 1000d);
                                                    break;
                                                }
                                                else if (j == nCouches - 1)
                                                {
                                                    W[(int)i] = TransChlorApi.Models.Functions.Water((decimal)Chydr[(int)i], HAncien[(int)i], ref T_new[(int)i], ref Tijd, ref tProt[nCouches], ref Vct[nCouches], ref Nct[nCouches], ref EC[nCouches], ref SAT[nCouches], ref Hydr[nCouches], ref ciment[nCouches], ref Wol);
                                                    Ph[(int)i] = (decimal)pPH;
                                                    Gamma[(int)i] = (float)(Math.Exp(aOH * (1d - Math.Pow(10d, (double)Ph[(int)i] - pPH))) * Math.Exp(EbG / R * (1d / (273.16d + (double)T_new[(int)i]) - (double)(1f / toG))) * ciment[nCouches] * Hydr[nCouches] * faG / 1000d);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            W[(int)i] = TransChlorApi.Models.Functions.Water((decimal)Chydr[(int)i], HAncien[(int)i], ref T_new[(int)i], ref Tijd, ref tProt[Boucle2], ref Vct[Boucle2], ref Nct[Boucle2], ref EC[Boucle2], ref SAT[Boucle2], ref Hydr[Boucle2], ref ciment[Boucle2], ref Wol);
                                            Ph[(int)i] = (decimal)pPH;
                                            Gamma[(int)i] = (float)(Math.Exp(aOH * (1d - Math.Pow(10d, (double)Ph[(int)i] - pPH))) * Math.Exp(EbG / R * (1d / (273.16d + (double)T_new[(int)i]) - (double)(1f / toG))) * ciment[Boucle2] * Hydr[Boucle2] * faG / 1000d);
                                        }  // 03.10.2023 fin


                                    }
                                    // Cion(Dofs + 1) = Cion(Dofs + 1) * Wsat * 35.453 / 58.443
                                    H_old[Dofs + 1] = (decimal)Chydr[(int)i];
                                    H_new[Dofs + 1] = (decimal)Chydr[Dofs];
                                    H_trial[Dofs + 1] = (decimal)Chydr[Dofs];
                                    Hold[Dofs + 1] = (decimal)Chydr[Dofs];
                                    HAncien[Dofs + 1] = (decimal)Chydr[Dofs];
                                    T_old[Dofs + 1] = (decimal)Ctherm[Dofs];
                                    T_new[Dofs + 1] = (decimal)Ctherm[Dofs];
                                    T_trial[Dofs + 1] = (decimal)Ctherm[Dofs];
                                    if (nCouches > 1) // 03.10.2023 d�but
                                    {
                                        W[Dofs + 1] = TransChlorApi.Models.Functions.Water((decimal)Chydr[Dofs], HAncien[Dofs], ref T_new[Dofs], ref Tijd, ref tProt[nCouches], ref Vct[nCouches], ref Nct[nCouches], ref EC[nCouches], ref SAT[nCouches], ref Hydr[nCouches], ref ciment[nCouches], ref Wol);
                                    }
                                    else
                                    {
                                        W[Dofs + 1] = TransChlorApi.Models.Functions.Water((decimal)Chydr[Dofs], HAncien[Dofs], ref T_new[Dofs], ref Tijd, ref tProt[Boucle2], ref Vct[Boucle2], ref Nct[Boucle2], ref EC[Boucle2], ref SAT[Boucle2], ref Hydr[Boucle2], ref ciment[Boucle2], ref Wol);
                                    }     // 03.10.2023 fin

                                    Hleft = 0m;
                                    Hright = 0m;
                                    ptest = false;

                                    var loopTo10 = (long)Dofs;
                                    for (i = 1L; i <= loopTo10; i++) // conditions aux limites sur les noeuds
                                    {
                                        C_old[(int)i] = TransChlorApi.Models.Functions.Cfree((decimal)Cion[(int)i], (decimal)Gamma[(int)i], W[(int)i]);
                                        C_new[(int)i] = C_old[(int)i];
                                        C_trial[(int)i] = C_old[(int)i];
                                        CT[(int)i] = (decimal)Cion[(int)i];
                                        if (PosProf[(int)i] < LgLim & i != 0L)
                                        {
                                            Hleft = Hleft + H_new[(int)i] * Le[(int)i];
                                            if (PosProf[(int)(i + 1L)] >= LgLim)
                                                Hleft = Hleft + H_new[(int)(i + 1L)] * (LgLim - PosProf[(int)i]);
                                        }
                                        if ((float)PosProf[(int)i] > Length - LgLim & i != Dofs + 1 & BDlibre == false)
                                        {
                                            if (ptest == false)
                                            {
                                                Hright = (decimal)((float)H_new[(int)i] * ((float)PosProf[(int)i] - Length + LgLim));
                                                ptest = true;
                                            }
                                            else
                                            {
                                                Hright = Hright + H_new[(int)i] * Le[(int)(i - 1L)];
                                            }
                                        }
                                    }

                                    Hright = Hright / LgLim; // Regle pb de Dcap des 2 cot�s
                                    Hleft = Hleft / LgLim; // Regle pb de Dcap des 2 cot�s

                                    int argCTmax = (int)Math.Round(CTmax);
                                    int argCLmax = (int)Math.Round(CLmax);
                                    float argGxc = (float)Gxc;
                                    float argDxc = (float)Dxc;
                                    //il dessine les graphiques ICI
                                    // frm.design(ref Wsat, ref H_old, ref W, ref T_old, ref C_new, ref CT, ref TempMin, ref TempMax, ref Length, ref PosProf, ref hMin, ref hEcart, ref wMin, ref wEcart, ref CTmin, ref CTecart, ref argCTmax, ref CLmin, ref CLecart, ref argCLmax, ref Tecart, ref Dofs, ref Tijd, ref argGxc, ref argDxc, ref Ph);
                                    CTmax = argCTmax;
                                    CLmax = argCLmax;
                                    Gxc = (double)argGxc;
                                    Dxc = (double)argDxc;

                                    // -------------------------------
                                    // start of computations
                                    affiche = 0d;
                                    Hteller = 0d;
                                    Wteller = 0d;
                                    CTteller = 0d;
                                    CLteller = 0d;
                                    Tteller = 0d;
                                    Carbteller = 0d;

                                    bool localReadExpo() { int argNbreEn = (int)GNbreEn; decimal argTempMin = (decimal)TempMin; int argNQUAL = NQUAL; bool argbordG = true; var ret = ReadExpo(ref FileGexpo[Boucle1], ref argNbreEn, ref GFiT, ref GTemperature, ref GHumidite, ref GSel, ref Msel, ref SAT, ref argTempMin, ref TempMax, ref TempEcart, ref nCouches, ref argNQUAL, ref argbordG); GNbreEn = argNbreEn; TempMin = (float)argTempMin; NQUAL = (short)argNQUAL; return ret; }

                                    if (localReadExpo() == false) // 03.10.2023 ajout nCouches + NQUAL + 0 + changer wsat en SAT()
                                    {
                                        // Interaction.MsgBox("ERROR: Exposition File not found!");
                                        Console.WriteLine("ERROR: Exposition File not found!");
                                        goto BreakBoucle1;
                                    }

                                    FiT = GFiT;
                                    NbreEn = GNbreEn;

                                    if (CultureInfo.CurrentCulture.CompareInfo.Compare(FileDexpo[Boucle1], "noFile", CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                                        BDlibre = true;

                                    if (BDlibre == false)
                                    {

                                        bool localReadExpo1() { int argNbreEn1 = (int)DNbreEn; decimal argTempMin1 = (decimal)TempMin; int argNQUAL1 = NQUAL; bool argbordG1 = false; var ret = ReadExpo(ref FileDexpo[Boucle1], ref argNbreEn1, ref DFiT, ref DTemperature, ref DHumidite, ref DSel, ref Msel, ref SAT, ref argTempMin1, ref TempMax, ref TempEcart, ref nCouches, ref argNQUAL1, ref argbordG1); DNbreEn = argNbreEn1; TempMin = (float)argTempMin1; NQUAL = (short)argNQUAL1; return ret; }

                                        if (localReadExpo1() == true)   // 03.10.2023 ajout nCouches + NQUAL + 0 + changer wsat en SAT()
                                        {

                                            if (GFiT != DFiT)
                                                // Interaction.MsgBox("fichier d'exposition incompatible", MsgBoxStyle.Information, "Avertissement");
                                                Console.WriteLine("fichier d'exposition incompatible");
                                            FiT = GFiT;

                                            if (GNbreEn >= DNbreEn)
                                            {
                                                NbreEn = GNbreEn;
                                            }
                                            else
                                            {
                                                NbreEn = DNbreEn;
                                            }

                                            Hsym = true;
                                            Ssym = true;
                                            Tsym = true;

                                            var loopTo11 = DNbreEn;
                                            for (j = 1L; j <= loopTo11; j++)
                                            {

                                                if ((float)Msel < DSel[(int)j])
                                                    Msel = (decimal)DSel[(int)j];
                                                if (DHumidite[(int)j] != GHumidite[(int)j])
                                                    Hsym = false;
                                                if (DSel[(int)j] != GSel[(int)j])
                                                    Ssym = false;
                                                if (DTemperature[(int)j] != GTemperature[(int)j])
                                                    Tsym = false;
                                            }
                                        }

                                        else
                                        {

                                            // Interaction.MsgBox("ERROR: Exposition File at Rigth not considered!");
                                            Console.WriteLine("ERROR: Exposition File at Rigth not considered!");
                                            BDlibre = true;

                                        }

                                    }

                                    TempEcart = TempMax - TempMin;

                                    if (TempEcart == 0f)       // cas d'essai isotherme
                                    {
                                        TempEcart = 0.1f;
                                        TempMax = 30f;
                                        TempMin = -10;
                                    }

                                    dTmax = 0.02f * TempEcart; // convergence conditions limites (temp�rature)
                                    testing3 = 0.0002f * TempEcart; // convergence it�ration (temp�rature)
                                    XdTmax = TempEcart / 3f; // grand saut du aux CL

                                    CumW = 0m;
                                    CumH = 0m;
                                    // CumClT = CDec(0)
                                    var loopTo12 = (long)(Dofs - 1);
                                    for (j = 1; j <= loopTo12; j++)       // calcul de la cumul�e de W
                                    {
                                        CumH = CumH + (H_new[(int)j] + H_new[(int)(j + 1L)]) * Le[(int)j] / 2m;
                                        CumW = CumW + Le[(int)j] * (W[(int)j] + W[(int)(j + 1)]) / 2m;
                                        // CumClT = CumClT + Le(j) * (CT(j) + CT(j + CShort(1))) / CDec(2)
                                    }
                                    CumH = CumH / (decimal)Length;
                                    CumW = CumW / (decimal)Length;
                                    // CumClT = CumClT / CDec(Length)

                                    GSTa = FileGexpo[Boucle1];
                                    DSTa = FileDexpo[Boucle1];

                                    // TransChlor.modDialog.FileOnly(ref GSTa);
                                    // TransChlor.modDialog.FileOnly(ref DSTa);
                                    // GSTa = Strings.Mid(GSTa, 6);
                                    // DSTa = Strings.Mid(DSTa, 6);
                                    // Num = (short)GSTa.Length;
                                    // GSTa = Strings.Left(GSTa, Num - 4);
                                    // Num = (short)DSTa.Length;
                                    // if (BDlibre == false)
                                    //     DSTa = Strings.Left(DSTa, Num - 4);
                                    GSTa = Path.GetFileNameWithoutExtension(GSTa);
                                    DSTa = Path.GetFileNameWithoutExtension(DSTa);

                                    string TitreGraph = "Panel with computed results, " + GSTa + ", " + DSTa + ", " + Filebeton[Boucle2] + ", " + FileProb;
                                    // frm.ModifyTitle(TitreGraph);

                                    // frm02.Text = "Panel with computed results, " & GSTa & ", " & DSTa & ", " & Filebeton(Boucle2) & ", " & FileProb 'Erreur VS2019 2020-04-24
                                    // nFic8 = CShort(FreeFile())      'prov
                                    // Dim File As String = "E:\Dotnet_TransChlor\bin\provisoire.txt"      'prov
                                    // FileOpen(CInt(nFic8), File, OpenMode.Output)  'prov
                                    // titre des fichiers r�sultats
                                    i = 0L;
                                    Tsec = 0L;
                                    i_day = 1L;

                                    // ''''''''''''''''''''''''''''''''' R�solution par �l�ments finis - boucle principale
                                    while (i_day >= 0L)
                                    {
                                        if (i_day > iGco * GNbreEn)
                                        {
                                            iG = 1L;
                                            iGco = iGco + 1L;
                                        }

                                        if (i_day > iDco * DNbreEn)
                                        {
                                            iD = 1L;
                                            iDco = iDco + 1L;
                                        }

                                        if (i_day == 1)
                                        {
                                            GTextold = (float)Ctherm[0];
                                            GHextold = (float)Chydr[0];
                                            GCFextold = (float)TransChlorApi.Models.Functions.Cfree((decimal)Cion[0],
                                                (decimal)Gamma[0], W[0]);
                                            DTextold = (float)Ctherm[0];
                                            DHextold = (float)Chydr[0];
                                            DCFextold = (float)TransChlorApi.Models.Functions.Cfree((decimal)Cion[0],
                                                (decimal)Gamma[0], W[0]);
                                            GTextern = (float)GTemperature[1];
                                            GHextern = GHumidite[1] / 100f;
                                            GCFextern = GSel[1] * (float)W[0];
                                            if (BDlibre == true)
                                            {
                                                DTextern = (float)Ctherm[Dofs];
                                                DHextern = (float)Chydr[Dofs];
                                                DCFextern = (float)TransChlorApi.Models.Functions.Cfree(
                                                    (decimal)Cion[Dofs], (decimal)Gamma[Dofs + 1], W[Dofs + 1]);
                                            }
                                            else
                                            {
                                                DTextern = (float)DTemperature[1];
                                                DHextern = DHumidite[1] / 100f;
                                                DCFextern = DSel[1] * (float)W[Dofs + 1];
                                            }
                                        }
                                        else
                                        {
                                            GTextold = GTextern;
                                            GHextold = GHextern;
                                            GCFextold = GCFextern;
                                            GTextern = (float)GTemperature[(int)iG];
                                            GHextern = GHumidite[(int)iG] / 100f; // [%]
                                            GCFextern = GSel[(int)iG] * (float)W[0]; // cF kg/m3 de b�ton
                                            DTextold = DTextern;
                                            DHextold = DHextern;
                                            DCFextold = DCFextern;
                                            if (BDlibre == true)
                                            {
                                                DTextern = (float)T_new[Dofs];
                                                DHextern = (float)(H_new[Dofs] / 100m); // [%]      
                                                DCFextern = (float)(C_trial[Dofs] * W[Dofs] / 1000m); // kg/m3 de b�ton
                                                DTextold = DTextern;
                                                DHextold = DHextern;
                                                DCFextold = DCFextern;
                                            }
                                            else
                                            {
                                                DTextern = (float)DTemperature[(int)iD];
                                                DHextern = DHumidite[(int)iD] / 100f; // [%]      
                                                DCFextern = DSel[(int)iD] * (float)W[Dofs + 1];
                                            } // kg/m3 de b�ton
                                        }

                                        if ((double)DHextern >= 0.99d & BDlibre == true)
                                            DHextern = 0.9f;
                                        GCTextern = (float)TransChlorApi.Models.Functions.Cfree((decimal)GCFextern,
                                            (decimal)Gamma[0], W[0]);
                                        GCTextold = (float)TransChlorApi.Models.Functions.Cfree((decimal)GCFextold,
                                            (decimal)Gamma[0], W[0]);
                                        DCTextern = (float)TransChlorApi.Models.Functions.Cfree((decimal)DCFextern,
                                            (decimal)Gamma[Dofs + 1], W[Dofs + 1]);
                                        DCTextold = (float)TransChlorApi.Models.Functions.Cfree((decimal)DCFextold,
                                            (decimal)Gamma[Dofs + 1], W[Dofs + 1]);

                                        // boundary conditions
                                        Again3: ;
                                        if (Math.Abs(GHextern - GHextold) > dHmax |
                                            Math.Abs(DHextern - DHextold) > dHmax |
                                            Math.Abs(GTextern - GTextold) > dTmax |
                                            Math.Abs(DTextern - DTextold) > dTmax |
                                            Math.Abs(GCTextern - GCTextold) > dCTmax |
                                            Math.Abs(DCTextern - DCTextold) > dCTmax | Cond01 > 0 |
                                            Math.Abs(GHextern - (float)H_new[1]) > XdHmax |
                                            Math.Abs(DHextern - (float)H_new[Dofs]) > XdHmax |
                                            Math.Abs(GTextern - (float)T_new[1]) > XdTmax |
                                            Math.Abs(DTextern - (float)T_new[Dofs]) > XdTmax |
                                            Math.Abs(GCTextern - (float)CT[2]) > XdCTmax |
                                            Math.Abs(DCTextern - (float)CT[Dofs - 1]) > XdCTmax |
                                            DeltaT !=
                                            FiT) // contr�le le delta h ou si le calcul est d�j� dans la boucle
                                        {
                                            if (Cond01 == 0)
                                            {
                                                Cond01 = 1;
                                                Interm[0] = (GHextern - GHextold) / dHmax;
                                                Interm[1] = (DHextern - DHextold) / dHmax;
                                                Interm[2] = (GTextern - GTextold) / dTmax;
                                                Interm[3] = (DTextern - DTextold) / dTmax;
                                                Interm[4] = (GCTextern - GCTextold) / dCTmax;
                                                Interm[5] = (DCTextern - DCTextold) / dCTmax;
                                                Interm[6] = 0f;
                                                if (Math.Abs(GHextern - (float)H_new[1]) > XdHmax |
                                                    Math.Abs(DHextern - (float)H_new[Dofs]) > XdHmax |
                                                    Math.Abs(GTextern - (float)T_new[1]) > XdTmax |
                                                    Math.Abs(DTextern - (float)T_new[Dofs]) > XdTmax |
                                                    Math.Abs(GCTextern - (float)CT[2]) > XdCTmax |
                                                    Math.Abs(DCTextern - (float)CT[Dofs - 1]) > XdCTmax)
                                                    Interm[6] = 1f;
                                                Interm[7] = FiT / DeltaT;
                                                if ((double)GHextern > 0.999d | (double)DHextern > 0.999d)
                                                    Interm[8] = 1f; // en cas de capillarit�
                                                for (j = 0; j <= 8; j++)
                                                {
                                                    if (Interm[(int)j] - Conversion.Int(Interm[(int)j]) ==
                                                        0f) // calcul le nombre de points interm�diaires
                                                    {
                                                        Interm[(int)j] = Math.Abs(Interm[(int)j]);
                                                    }
                                                    else
                                                    {
                                                        Interm[(int)j] = Conversion.Int(Math.Abs(Interm[(int)j])) + 1f;
                                                    }
                                                }

                                                Pinterm = 0;
                                                for (j = 0; j <= 8; j++)
                                                {
                                                    if (Interm[(int)j] > Pinterm)
                                                        Pinterm = (short)Math.Round(Interm[(int)j]);
                                                }

                                                if (PintermManual > 0)
                                                    Pinterm = (short)PintermManual;
                                                // If (GHextern > 0.999 And GHextold < 0.999) Or (DHextern > 0.999 And DHextold < 0.999) Then Pinterm = 1 ' en cas de capillarit�

                                                GdelTemp = (GTextern - GTextold) / Pinterm; // calcul de delta T
                                                GdelH = (GHextern - GHextold) / Pinterm; // calcul de delta H
                                                GdelCF = (GCFextern - GCFextold) / Pinterm; // calcul de delta CF
                                                DdelTemp = (DTextern - DTextold) / Pinterm; // calcul de delta T
                                                DdelH = (DHextern - DHextold) / Pinterm; // calcul de delta H
                                                DdelCF = (DCFextern - DCFextold) / Pinterm; // calcul de delta CF
                                                DeltaT = FiT / Pinterm; // calcul de delta T
                                            }

                                            GTextern = GTextold + GdelTemp * Cond01;
                                            GHextern = GHextold + GdelH * Cond01;
                                            GCFextern = GCFextold + GdelCF * Cond01;
                                            DTextern = DTextold + DdelTemp * Cond01;
                                            DHextern = DHextold + DdelH * Cond01;
                                            DCFextern = DCFextold + DdelCF * Cond01;
                                            Tijd = (i + (decimal)DeltaT * Cond01 / 3600m) / 24m; // tijd in days
                                            Cond01 = (short)(Cond01 + 1);
                                            if (Cond01 > Pinterm)
                                            {
                                                Pinterm = 0;
                                                Cond01 = 0;
                                                DeltaT = DelT;
                                                // i_hour = i_hour + 1
                                            }
                                        }

                                        if (Cond01 == 0)
                                        {
                                            Tsec = Tsec + (long)Math.Round(FiT);
                                            i = (long)Math.Round(Tsec / (double)3600L); // heures
                                            Tijd = i / 24m; // tijd in days
                                        }

                                        f2 = (decimal)DeltaT / 2.0m; // time integration constant

                                        // transport thermique
                                        // compose lhs and rhs
                                        // If Tijd > 221.78 Then Stop
                                        Again4: ;
                                        var loopTo28 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo28; j++) // initialisation
                                        {
                                            LHS[1, (int)j] = 0.0m;
                                            LHS[2, (int)j] = 0.0m;
                                            RHS[(int)j] = 0.0m;
                                        }

                                        var loopTo29 = (long)Dofs;
                                        for (j = 0; j <= loopTo29; j++) // construction de la matrice LHS . h = RHS
                                        {
                                            f1 = Le[(int)j] / 3.0m;
                                            if (nCouches > 1) // 03.10.2023 d�but
                                            {
                                                var loopTo30 = (long)(nCouches - 1);
                                                for (k = 1L; k <= loopTo30; k++)
                                                {
                                                    if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                    {
                                                        f3 = f2 * TransChlorApi.Models.Functions.MT(ref qGran[(int)k],
                                                            ref capCal, ref W[(int)j], ref ciment[(int)k],
                                                            ref Hydr[(int)k], ref T_old[(int)j]) / Le[(int)j];
                                                        if (j == 0 | j == Dofs)
                                                            f3 = f3 * (decimal)LambdaT[(int)k];
                                                        break;
                                                    }
                                                    else if (k == nCouches - 1)
                                                    {
                                                        f3 = f2 * TransChlorApi.Models.Functions.MT(ref qGran[nCouches],
                                                            ref capCal, ref W[(int)j], ref ciment[nCouches],
                                                            ref Hydr[nCouches], ref T_old[(int)j]) / Le[(int)j];
                                                        if (j == 0 | j == Dofs)
                                                            f3 = f3 * (decimal)LambdaT[nCouches];
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                f3 = f2 * TransChlorApi.Models.Functions.MT(ref qGran[Boucle2],
                                                    ref capCal, ref W[(int)j], ref ciment[Boucle2], ref Hydr[Boucle2],
                                                    ref T_old[(int)j]) / Le[(int)j];
                                                if (j == 0 | j == Dofs)
                                                    f3 = f3 * (decimal)LambdaT[Boucle2];
                                            } // 03.10.2023 fin

                                            LHS[1, (int)j] = LHS[1, (int)j] + f1 + f3;
                                            LHS[2, (int)j] = LHS[2, (int)j] + f1 / 2.0m - f3;
                                            LHS[1, (int)(j + 1)] = LHS[1, (int)(j + 1)] + f1 + f3;
                                            C11 = f1 - f3;
                                            C12 = f1 / 2.0m + f3;
                                            RHS[(int)j] = RHS[(int)j] + C11 * T_old[(int)j] + C12 * T_old[(int)(j + 1)];
                                            RHS[(int)(j + 1)] = RHS[(int)(j + 1)] + C12 * T_old[(int)j] +
                                                                C11 * T_old[(int)(j + 1)];
                                        }

                                        RHS[0] = (decimal)GTextern; // condition aux limites
                                        LHS[1, 0] = 1.0m;
                                        RHS[1] = RHS[1] - LHS[2, 0] * (decimal)GTextern;
                                        LHS[2, 0] = 0.0m;
                                        // bord 2
                                        if (BDlibre == false)
                                        {
                                            RHS[Dofs + 1] = (decimal)DTextern;
                                            LHS[1, Dofs + 1] = 1.0m;
                                            RHS[Dofs] = RHS[Dofs] - LHS[2, Dofs] * (decimal)DTextern;
                                            LHS[2, Dofs] = 0.0m;
                                        }

                                        // solve system of equations
                                        var loopTo31 = (long)(Dofs + 1);
                                        for (j = 1; j <= loopTo31; j++)
                                        {
                                            LHS[1, (int)j] = LHS[1, (int)j] - LHS[2, (int)(j - 1)] *
                                                LHS[2, (int)(j - 1)] / LHS[1, (int)(j - 1)];
                                            RHS[(int)j] = RHS[(int)j] - RHS[(int)(j - 1)] * LHS[2, (int)(j - 1)] /
                                                LHS[1, (int)(j - 1)];
                                        }

                                        T_trial[Dofs + 1] = RHS[Dofs + 1] / LHS[1, Dofs + 1];
                                        for (j = Dofs; j >= 0; j += -1)
                                            T_trial[(int)j] = (RHS[(int)j] - LHS[2, (int)j] * T_trial[(int)(j + 1)]) /
                                                              LHS[1, (int)j];
                                        // check on convergence
                                        test = 0.0m;
                                        var loopTo32 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo32; j++)
                                        {
                                            if (Math.Abs(T_trial[(int)j] - T_new[(int)j]) > test)
                                            {
                                                test = Math.Abs(T_trial[(int)j] - T_new[(int)j]);
                                            }

                                            T_trial[(int)j] = 0.6m * T_new[(int)j] + 0.4m * T_trial[(int)j];
                                            T_new[(int)j] = T_trial[(int)j];
                                        }

                                        if (test > (decimal)testing3)
                                            goto Again4;
                                        // update variables
                                        Tleft = 0m;
                                        Tright = 0m;
                                        ptest = false;

                                        var loopTo33 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo33; j++)
                                        {
                                            if (j != Dofs + 1)
                                            {
                                                if (Math.Abs(T_new[(int)(j + 1L)] - T_new[(int)j]) >= 2m)
                                                {
                                                    T_new[(int)j] = (decimal)GTextern;
                                                    T_new[(int)(j + 1L)] = (decimal)GTextern;
                                                }
                                            }

                                            T_old[(int)j] = T_new[(int)j];
                                            if (PosProf[(int)j] < LgLim & j != 0L & j != Dofs + 1)
                                            {
                                                Tleft = Tleft + T_new[(int)j] * Le[(int)j];
                                                if (PosProf[(int)(j + 1L)] >= LgLim)
                                                    Tleft = Tleft + T_new[(int)(j + 1L)] * (LgLim - PosProf[(int)j]);
                                            }

                                            if ((float)PosProf[(int)j] > Length - LgLim & j != Dofs + 1 &
                                                BDlibre == false)
                                            {
                                                if (ptest == false)
                                                {
                                                    Tright = (decimal)((float)T_new[(int)j] *
                                                                       ((float)PosProf[(int)j] - Length + LgLim));
                                                    ptest = true;
                                                }
                                                else
                                                {
                                                    Tright = Tright + T_new[(int)j] * Le[(int)(j - 1L)];
                                                }
                                            }
                                        }

                                        if (Tsym == true)
                                        {
                                            var loopTo34 = (long)(short)Math.Round((Dofs + 1) / 2d);
                                            for (j = 0; j <= loopTo34; j++)
                                            {
                                                T_new[(int)j] = (T_new[(int)j] + T_new[(int)(Dofs + 1 - j)]) / 2m;
                                                T_new[(int)(Dofs + 1 - j)] = T_new[(int)j];
                                                T_old[(int)j] = T_new[(int)j];
                                                T_old[(int)(Dofs + 1 - j)] = T_old[(int)j];
                                            }
                                        }

                                        Tright = Tright / LgLim;
                                        Tleft = Tleft / LgLim;

                                        // Tranport hydrique
                                        // compose lhs and rhs
                                        Again1: ;
                                        var loopTo35 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo35; j++) // initialisation
                                        {
                                            LHS[0, (int)j] = 0.0m;
                                            LHS[1, (int)j] = 0.0m;
                                            LHS[2, (int)j] = 0.0m;
                                            RHS[(int)j] = 0.0m;
                                        }

                                        Gcptj = 0L;
                                        Dcptj = Dofs + 1;
                                        var loopTo36 = (long)Dofs;
                                        for (j = 1L; j <= loopTo36; j++)
                                        {
                                            if ((float)H_trial[(int)j] < H_snap | Gcptj != 0L)
                                            {
                                                if (Gcptj == 0L)
                                                    Gcptj = j;
                                                if (PosProf[(int)j] > PosProf[(int)Gcptj] + 5m)
                                                {
                                                    Gcptj = j - 1L;
                                                    break;
                                                }
                                            }

                                            if (j == Dofs)
                                                Gcptj = Dofs + 1;
                                        }

                                        if (BDlibre == false)
                                        {
                                            for (j = Dofs; j >= 1L; j += -1)
                                            {
                                                if ((float)H_trial[(int)j] < H_snap | Dcptj != Dofs + 1)
                                                {
                                                    if (Dcptj == Dofs + 1)
                                                        Dcptj = j;
                                                    if (PosProf[(int)j] < PosProf[(int)Dcptj] - 5m)
                                                    {
                                                        Dcptj = j + 1L;
                                                        break;
                                                    }
                                                }

                                                if (j == 1L)
                                                    Dcptj = 0L;
                                            }
                                        }

                                        // calcul du coefficient de capillarit�
                                        if (nCouches == 1) // 03.10.2023 d�but
                                        {
                                            parA = (decimal)(0.0000624775d * Math.Pow(EsC[Boucle2], 2d) -
                                                0.00010384d * EsC[Boucle2] + 0.0000300346d);
                                            parA1 = (decimal)(0.000000278804d * Math.Pow((double)Tleft, 3d) -
                                                              0.00000735523d * Math.Pow((double)Tleft, 2d) -
                                                              0.000278074d * (double)Tleft - 0.012309435d);
                                            parB1 = (decimal)(-0.000000303977d * Math.Pow((double)Tleft, 3d) +
                                                              0.00000797499d * Math.Pow((double)Tleft, 2d) +
                                                              0.00033679d * (double)Tleft + 0.017793224d);
                                            parC1 = (decimal)(0.0000000800226d * Math.Pow((double)Tleft, 3d) -
                                                              0.00000226181d * Math.Pow((double)Tleft, 2d) -
                                                              0.0000897841d * (double)Tleft - 0.004607865d);
                                            parB = (decimal)((double)parA1 * Math.Pow(EsC[Boucle2], 2d) +
                                                             (double)((float)parB1 * EsC[Boucle2]) + (double)parC1);
                                            DcapLeft = parA * Hleft * 100m + parB;
                                            if ((double)DcapLeft < 0.000025d)
                                                DcapLeft = 0.000025m;
                                            if ((double)DcapLeft > 0.009d)
                                                DcapLeft = 0.009m;
                                            if (BDlibre == false)
                                            {
                                                parA1 = (decimal)(0.000000278804d * Math.Pow((double)Tright, 3d) -
                                                                  0.00000735523d * Math.Pow((double)Tright, 2d) -
                                                                  0.000278074d * (double)Tright - 0.012309435d);
                                                parB1 = (decimal)(-0.000000303977d * Math.Pow((double)Tright, 3d) +
                                                                  0.00000797499d * Math.Pow((double)Tright, 2d) +
                                                                  0.00033679d * (double)Tright + 0.017793224d);
                                                parC1 = (decimal)(0.0000000800226d * Math.Pow((double)Tright, 3d) -
                                                                  0.00000226181d * Math.Pow((double)Tright, 2d) -
                                                                  0.0000897841d * (double)Tright - 0.004607865d);
                                                parB = (decimal)((double)parA1 * Math.Pow(EsC[Boucle2], 2d) +
                                                                 (double)((float)parB1 * EsC[Boucle2]) + (double)parC1);
                                                DcapRight = parA * Hright * 100m + parB;
                                                if ((double)DcapRight < 0.000025d)
                                                    DcapRight = 0.000025m;
                                                if ((double)DcapRight > 0.009d)
                                                    DcapRight = 0.009m;
                                            }
                                        } // 03.10.2023 fin

                                        var loopTo37 = (long)Dofs;
                                        for (j = 0; j <= loopTo37; j++) // construction de la matrice LHS . h = RHS
                                        {
                                            if (nCouches > 1) // 03.10.2023 d�but
                                            {
                                                var loopTo38 = (long)(nCouches - 1);
                                                for (k = 1L; k <= loopTo38; k++)
                                                {
                                                    if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                    {
                                                        parA = (decimal)(0.0000624775d * Math.Pow(EsC[(int)k], 2d) -
                                                            0.00010384d * EsC[(int)k] + 0.0000300346d);
                                                        parA1 =
                                                            (decimal)(0.000000278804d * Math.Pow((double)Tleft, 3d) -
                                                                      0.00000735523d * Math.Pow((double)Tleft, 2d) -
                                                                      0.000278074d * (double)Tleft - 0.012309435d);
                                                        parB1 =
                                                            (decimal)(-0.000000303977d * Math.Pow((double)Tleft, 3d) +
                                                                      0.00000797499d * Math.Pow((double)Tleft, 2d) +
                                                                      0.00033679d * (double)Tleft + 0.017793224d);
                                                        parC1 =
                                                            (decimal)(0.0000000800226d * Math.Pow((double)Tleft, 3d) -
                                                                      0.00000226181d * Math.Pow((double)Tleft, 2d) -
                                                                      0.0000897841d * (double)Tleft - 0.004607865d);
                                                        parB = (decimal)((double)parA1 * Math.Pow(EsC[(int)k], 2d) +
                                                                         (double)((float)parB1 * EsC[(int)k]) +
                                                                         (double)parC1);
                                                        DcapLeft = parA * Hleft * 100m + parB;
                                                        if ((double)DcapLeft < 0.000025d)
                                                            DcapLeft = 0.000025m;
                                                        if ((double)DcapLeft > 0.009d)
                                                            DcapLeft = 0.009m;
                                                        if (BDlibre == false)
                                                        {
                                                            parA1 = (decimal)(0.000000278804d *
                                                                              Math.Pow((double)Tright, 3d) -
                                                                              0.00000735523d * Math.Pow((double)Tright,
                                                                                  2d) -
                                                                              0.000278074d * (double)Tright -
                                                                              0.012309435d);
                                                            parB1 = (decimal)(-0.000000303977d *
                                                                              Math.Pow((double)Tright, 3d) +
                                                                              0.00000797499d * Math.Pow((double)Tright,
                                                                                  2d) +
                                                                              0.00033679d * (double)Tright +
                                                                              0.017793224d);
                                                            parC1 = (decimal)(0.0000000800226d *
                                                                              Math.Pow((double)Tright, 3d) -
                                                                              0.00000226181d * Math.Pow((double)Tright,
                                                                                  2d) -
                                                                              0.0000897841d * (double)Tright -
                                                                              0.004607865d);
                                                            parB = (decimal)((double)parA1 * Math.Pow(EsC[(int)k], 2d) +
                                                                             (double)((float)parB1 * EsC[(int)k]) +
                                                                             (double)parC1);
                                                            DcapRight = parA * Hright * 100m + parB;
                                                            if ((double)DcapRight < 0.000025d)
                                                                DcapRight = 0.000025m;
                                                            if ((double)DcapRight > 0.009d)
                                                                DcapRight = 0.009m;
                                                        }

                                                        Ha = (H_old[(int)j] + H_old[(int)(j + 1)] + H_trial[(int)j] +
                                                              H_trial[(int)(j + 1)]) / 4.0m;
                                                        f1 = Le[(int)j] / 3.0m;

                                                        decimal localMDCdiff()
                                                        {
                                                            var tmp = CoefHr;
                                                            float argcoef = (float)tmp[Boucle3];
                                                            var ret = TransChlorApi.Models.Functions.MDCdiff(ref Ha,
                                                                ref argcoef, ref PD[(int)k], ref Hc, ref aa,
                                                                ref ED[(int)k], ref ToHydr[(int)k], ref T_old[(int)j]);
                                                            tmp[Boucle3] = argcoef;
                                                            return ret;
                                                        }

                                                        f3 = f2 * localMDCdiff() / Le[(int)j];
                                                        break;
                                                    }
                                                    else if (k == nCouches - 1)
                                                    {
                                                        parA = (decimal)(0.0000624775d * Math.Pow(EsC[nCouches], 2d) -
                                                            0.00010384d * EsC[nCouches] + 0.0000300346d);
                                                        parA1 =
                                                            (decimal)(0.000000278804d * Math.Pow((double)Tleft, 3d) -
                                                                      0.00000735523d * Math.Pow((double)Tleft, 2d) -
                                                                      0.000278074d * (double)Tleft - 0.012309435d);
                                                        parB1 =
                                                            (decimal)(-0.000000303977d * Math.Pow((double)Tleft, 3d) +
                                                                      0.00000797499d * Math.Pow((double)Tleft, 2d) +
                                                                      0.00033679d * (double)Tleft + 0.017793224d);
                                                        parC1 =
                                                            (decimal)(0.0000000800226d * Math.Pow((double)Tleft, 3d) -
                                                                      0.00000226181d * Math.Pow((double)Tleft, 2d) -
                                                                      0.0000897841d * (double)Tleft - 0.004607865d);
                                                        parB = (decimal)((double)parA1 * Math.Pow(EsC[nCouches], 2d) +
                                                                         (double)((float)parB1 * EsC[nCouches]) +
                                                                         (double)parC1);
                                                        DcapLeft = parA * Hleft * 100m + parB;
                                                        if ((double)DcapLeft < 0.000025d)
                                                            DcapLeft = 0.000025m;
                                                        if ((double)DcapLeft > 0.009d)
                                                            DcapLeft = 0.009m;
                                                        if (BDlibre == false)
                                                        {
                                                            parA1 = (decimal)(0.000000278804d *
                                                                              Math.Pow((double)Tright, 3d) -
                                                                              0.00000735523d * Math.Pow((double)Tright,
                                                                                  2d) -
                                                                              0.000278074d * (double)Tright -
                                                                              0.012309435d);
                                                            parB1 = (decimal)(-0.000000303977d *
                                                                              Math.Pow((double)Tright, 3d) +
                                                                              0.00000797499d * Math.Pow((double)Tright,
                                                                                  2d) +
                                                                              0.00033679d * (double)Tright +
                                                                              0.017793224d);
                                                            parC1 = (decimal)(0.0000000800226d *
                                                                              Math.Pow((double)Tright, 3d) -
                                                                              0.00000226181d * Math.Pow((double)Tright,
                                                                                  2d) -
                                                                              0.0000897841d * (double)Tright -
                                                                              0.004607865d);
                                                            parB =
                                                                (decimal)((double)parA1 * Math.Pow(EsC[nCouches], 2d) +
                                                                          (double)((float)parB1 * EsC[nCouches]) +
                                                                          (double)parC1);
                                                            DcapRight = parA * Hright * 100m + parB;
                                                            if ((double)DcapRight < 0.000025d)
                                                                DcapRight = 0.000025m;
                                                            if ((double)DcapRight > 0.009d)
                                                                DcapRight = 0.009m;
                                                        }

                                                        Ha = (H_old[(int)j] + H_old[(int)(j + 1)] + H_trial[(int)j] +
                                                              H_trial[(int)(j + 1)]) / 4.0m;
                                                        f1 = Le[(int)j] / 3.0m;

                                                        decimal localMDCdiff1()
                                                        {
                                                            var tmp1 = CoefHr;
                                                            float argcoef1 = (float)tmp1[Boucle3];
                                                            var ret = TransChlorApi.Models.Functions.MDCdiff(ref Ha,
                                                                ref argcoef1, ref PD[nCouches], ref Hc, ref aa,
                                                                ref ED[nCouches], ref ToHydr[nCouches],
                                                                ref T_old[(int)j]);
                                                            tmp1[Boucle3] = argcoef1;
                                                            return ret;
                                                        }

                                                        f3 = f2 * localMDCdiff1() / Le[(int)j];
                                                        break;
                                                    }
                                                }
                                            }

                                            if (nCouches == 1)
                                            {
                                                Ha = (H_old[(int)j] + H_old[(int)(j + 1)] + H_trial[(int)j] +
                                                      H_trial[(int)(j + 1)]) / 4.0m;
                                                f1 = Le[(int)j] / 3.0m;

                                                decimal localMDCdiff2()
                                                {
                                                    var tmp2 = CoefHr;
                                                    float argcoef2 = (float)tmp2[Boucle3];
                                                    var ret = TransChlorApi.Models.Functions.MDCdiff(ref Ha,
                                                        ref argcoef2, ref PD[Bou2], ref Hc, ref aa, ref ED[Bou2],
                                                        ref ToHydr[Bou2], ref T_old[(int)j]);
                                                    tmp2[Boucle3] = argcoef2;
                                                    return ret;
                                                }

                                                f3 = f2 * localMDCdiff2() / Le[(int)j];
                                            } // 03.10.2023 fin

                                            if (j < Gcptj | j > Dcptj)
                                            {
                                                decimal localMDCcap()
                                                {
                                                    var tmp3 = CoefCap;
                                                    float argcoef3 = (float)tmp3[Boucle4];
                                                    bool argQuest = true;
                                                    decimal argLength = (decimal)Length;
                                                    decimal argLgLim = LgLim;
                                                    var ret = TransChlorApi.Models.Functions.MDCcap(ref GHextern,
                                                        ref DHextern, ref argcoef3, ref DeltaT, ref tPrec, ref j,
                                                        ref Tijd, ref tijdOld, ref argQuest, ref ab, ref tc,
                                                        ref DcapLeft, DcapRight, ref BDlibre, ref PosProf[(int)j],
                                                        ref argLength, ref argLgLim, ref ImpHydr);
                                                    tmp3[Boucle4] = argcoef3;
                                                    Length = (float)argLength;
                                                    LgLim = (int)Math.Round(argLgLim);
                                                    return ret;
                                                }

                                                f5 = localMDCcap() * f2 / 2m;
                                            }
                                            else
                                            {
                                                f5 = 0m;
                                            }

                                            if (j == 0 | j == Dofs)
                                            {
                                                if (nCouches > 1) // 03.10.2023 d�but
                                                {
                                                    var loopTo39 = (long)(nCouches - 1);
                                                    for (k = 1L; k <= loopTo39; k++)
                                                    {
                                                        if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                        {
                                                            f3 = f3 * (decimal)LambdaH[(int)k];
                                                            f5 = f5 * (decimal)LambdaH[(int)k];
                                                            break;
                                                        }
                                                        else if (k == nCouches - 1)
                                                        {
                                                            f3 = f3 * (decimal)LambdaH[nCouches];
                                                            f5 = f5 * (decimal)LambdaH[nCouches];
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    f3 = f3 * (decimal)LambdaH[Boucle2];
                                                    f5 = f5 * (decimal)LambdaH[Boucle2];
                                                } // 03.10.2023 fin
                                            }

                                            LHS[0, (int)j] = LHS[0, (int)j] + f1 / 2.0m - f3 + f5;
                                            LHS[1, (int)j] = LHS[1, (int)j] + f1 + f3 - f5;
                                            LHS[2, (int)j] = LHS[2, (int)j] + f1 / 2.0m - f3 - f5;
                                            LHS[1, (int)(j + 1)] = LHS[1, (int)(j + 1)] + f1 + f3 + f5;
                                            C11 = f1 - f3 + f5;
                                            C12 = f1 / 2.0m + f3 - f5;
                                            RHS[(int)j] = RHS[(int)j] + C11 * H_old[(int)j] + C12 * H_old[(int)(j + 1)];
                                            C11 = f1 - f3 - f5;
                                            C12 = f1 / 2.0m + f3 + f5;
                                            RHS[(int)(j + 1)] = RHS[(int)(j + 1)] + C12 * H_old[(int)j] +
                                                                C11 * H_old[(int)(j + 1)];
                                        }

                                        RHS[0] = (decimal)GHextern; // condition aux limites
                                        LHS[1, 0] = 1.0m;
                                        RHS[1] = (decimal)((float)RHS[1] - (float)LHS[2, 0] * GHextern);
                                        LHS[2, 0] = 0.0m;
                                        LHS[0, 0] = 0.0m;
                                        // bord 2
                                        if (BDlibre == false)
                                        {
                                            RHS[Dofs + 1] = (decimal)DHextern;
                                            LHS[1, Dofs + 1] = 1m;
                                            RHS[Dofs] = RHS[Dofs] - LHS[0, Dofs] * (decimal)DHextern;
                                            LHS[0, Dofs] = 0m;
                                            LHS[2, Dofs] = 0m;
                                        }

                                        // solve system of equations
                                        var loopTo40 = (long)(Dofs + 1);
                                        for (j = 1; j <= loopTo40; j++)
                                        {
                                            LHS[1, (int)j] = LHS[1, (int)j] - LHS[2, (int)(j - 1)] *
                                                LHS[0, (int)(j - 1)] / LHS[1, (int)(j - 1)];
                                            RHS[(int)j] = RHS[(int)j] - RHS[(int)(j - 1)] * LHS[2, (int)(j - 1)] /
                                                LHS[1, (int)(j - 1)];
                                        }

                                        H_trial[Dofs + 1] = RHS[Dofs + 1] / LHS[1, Dofs + 1];
                                        for (j = Dofs; j >= 0; j += -1)
                                            H_trial[(int)j] = (RHS[(int)j] - LHS[0, (int)j] * H_trial[(int)(j + 1)]) /
                                                              LHS[1, (int)j];
                                        H_trial[0] = (decimal)GHextern;
                                        if (GHextern > 0.999f)
                                            H_trial[1] = 1m; // court-circuit en cas capillarit�
                                        if (BDlibre == false)
                                        {
                                            H_trial[Dofs + 1] = (decimal)DHextern;
                                            if (DHextern > 0.999f)
                                                H_trial[Dofs] = 1m;
                                        }

                                        // check on convergence
                                        test = 0.0m;
                                        var loopTo41 = (long)(Dofs + 1);
                                        for (j = 0L; j <= loopTo41; j++)
                                        {
                                            if (Math.Abs(H_trial[(int)j] - H_new[(int)j]) > test)
                                            {
                                                test = Math.Abs(H_trial[(int)j] - H_new[(int)j]);
                                            }

                                            H_trial[(int)j] = 0.6m * H_new[(int)j] + 0.4m * H_trial[(int)j];
                                            H_new[(int)j] = H_trial[(int)j];
                                        }

                                        H_new[0] = (decimal)GHextern;
                                        if (BDlibre == false)
                                            H_new[Dofs + 1] = (decimal)DHextern;
                                        if (GHextern > 0.999f)
                                            H_new[1] = 1m; // court-circuit en cas capillarit�
                                        if (BDlibre == false)
                                        {
                                            if (DHextern > 0.999f)
                                                H_new[Dofs] = 1m;
                                        }

                                        if (testOld <= test)
                                            test = (decimal)(testing1 / 10f); // en cas de non convergence
                                        testOld = test;
                                        if (test > (decimal)testing1)
                                            goto Again1;
                                        // update variables
                                        TW = true;
                                        Hleft = 0m;
                                        Hright = 0m;
                                        ptest = false;
                                        var loopTo42 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo42; j++)
                                        {
                                            if (H_new[(int)j] > 1m)
                                                H_new[(int)j] = 1m; // probl�me num�rique
                                            if (H_new[(int)j] < 0m)
                                                H_new[(int)j] = 0m;
                                            if (i_day == 1L)
                                                W_old[(int)j] = W[(int)j];
                                            if (nCouches > 1) // 03.10.2023 d�but
                                            {
                                                var loopTo43 = (long)(nCouches - 1);
                                                for (k = 1L; k <= loopTo43; k++)
                                                {
                                                    if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                    {
                                                        W[(int)j] = TransChlorApi.Models.Functions.Water(H_new[(int)j],
                                                            HAncien[(int)j], ref T_new[(int)j], ref Tijd,
                                                            ref tProt[(int)k], ref Vct[(int)k], ref Nct[(int)k],
                                                            ref EC[(int)k], ref SAT[(int)k], ref Hydr[(int)k],
                                                            ref ciment[(int)k], ref Wol);
                                                        if (W[(int)j] < 0m | (float)W[(int)j] > SAT[(int)k])
                                                            W[(int)j] = (decimal)(SAT[(int)k] * (float)H_new[(int)j]);
                                                        break;
                                                    }
                                                    else if (k == nCouches - 1)
                                                    {
                                                        W[(int)j] = TransChlorApi.Models.Functions.Water(H_new[(int)j],
                                                            HAncien[(int)j], ref T_new[(int)j], ref Tijd,
                                                            ref tProt[nCouches], ref Vct[nCouches], ref Nct[nCouches],
                                                            ref EC[nCouches], ref SAT[nCouches], ref Hydr[nCouches],
                                                            ref ciment[nCouches], ref Wol);
                                                        if (W[(int)j] < 0m | (float)W[(int)j] > SAT[nCouches])
                                                            W[(int)j] = (decimal)(SAT[nCouches] * (float)H_new[(int)j]);
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                W[(int)j] = TransChlorApi.Models.Functions.Water(H_new[(int)j],
                                                    HAncien[(int)j], ref T_new[(int)j], ref Tijd, ref tProt[Boucle2],
                                                    ref Vct[Boucle2], ref Nct[Boucle2], ref EC[Boucle2],
                                                    ref SAT[Boucle2], ref Hydr[Boucle2], ref ciment[Boucle2], ref Wol);
                                                if (W[(int)j] < 0m | (float)W[(int)j] > SAT[Boucle2])
                                                    W[(int)j] = (decimal)(SAT[Boucle2] * (float)H_new[(int)j]);
                                            } // 03.10.2023 fin

                                            if (j >= Dofs / 3d & j <= 2 * Dofs / 3d &
                                                Math.Abs(W_old[(int)j] - W[(int)j]) < 5m)
                                                TW = false;
                                            if (Cond01 == 0)
                                                HAncien[(int)j] = H_new[(int)j];
                                            if (PosProf[(int)j] < LgLim & j != 0L & j != Dofs + 1)
                                            {
                                                Hleft = Hleft + H_new[(int)j] * Le[(int)j];
                                                if (PosProf[(int)(j + 1L)] >= LgLim)
                                                    Hleft = Hleft + H_new[(int)(j + 1L)] * (LgLim - PosProf[(int)j]);
                                            }

                                            if ((float)PosProf[(int)j] > Length - LgLim & j != Dofs + 1 &
                                                BDlibre == false)
                                            {
                                                if (ptest == false)
                                                {
                                                    Hright = (decimal)((float)H_new[(int)j] *
                                                                       ((float)PosProf[(int)j] - Length + LgLim));
                                                    ptest = true;
                                                }
                                                else
                                                {
                                                    Hright = Hright + H_new[(int)j] * Le[(int)(j - 1L)];
                                                }
                                            }
                                        }

                                        Hright = Hright / LgLim;
                                        Hleft = Hleft / LgLim;

                                        if (Hsym == true)
                                        {
                                            var loopTo44 = (long)(short)Math.Round((Dofs + 1) / 2d);
                                            for (j = 0; j <= loopTo44; j++)
                                            {
                                                H_new[(int)j] = (H_new[(int)j] + H_new[(int)(Dofs + 1 - j)]) / 2m;
                                                H_new[(int)(Dofs + 1 - j)] = H_new[(int)j];
                                                H_old[(int)j] = H_new[(int)j];
                                                H_old[(int)(Dofs + 1 - j)] = H_old[(int)j];
                                            }
                                        }

                                        if (TW == true)
                                        {
                                            var loopTo45 = (long)(Dofs - 1);
                                            for (j = 2; j <= loopTo45; j++)
                                            {
                                                W[(int)j] = (decimal)((float)W_old[(int)j] +
                                                                      (float)(W[(int)j] - W_old[(int)j]) /
                                                                      (DeltaT / 36f));
                                                if (nCouches > 1) // 03.10.2023 d�but
                                                {
                                                    var loopTo46 = (long)(nCouches - 1);
                                                    for (k = 1L; k <= loopTo46; k++)
                                                    {
                                                        if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                        {
                                                            if (W[(int)j] < 0m | (float)W[(int)j] > SAT[(int)k])
                                                                W[(int)j] =
                                                                    (decimal)(SAT[(int)k] * (float)H_new[(int)j]);
                                                            break;
                                                        }
                                                        else if (k == nCouches - 1)
                                                        {
                                                            if (W[(int)j] < 0m | (float)W[(int)j] > SAT[nCouches])
                                                                W[(int)j] = (decimal)(SAT[nCouches] *
                                                                    (float)H_new[(int)j]);
                                                            break;
                                                        }
                                                    }
                                                }
                                                else if (W[(int)j] < 0m | (float)W[(int)j] > SAT[Boucle2])
                                                    W[(int)j] = (decimal)(SAT[Boucle2] *
                                                                          (float)H_new[(int)j]); // 03.10.2023 fin
                                            }
                                        }

                                        CumW = 0m;
                                        CumH = 0m;
                                        var loopTo47 = (long)(Dofs - 1);
                                        for (j = 1; j <= loopTo47; j++)
                                        {
                                            CumH = CumH + (H_new[(int)j] + H_new[(int)(j + 1L)]) * Le[(int)j] / 2m;
                                            CumW = CumW + Le[(int)j] * (W[(int)j] + W[(int)(j + 1L)]) / 2m;
                                            W_old[(int)j] = W[(int)j];
                                        }

                                        W_old[0] = W[0];
                                        W_old[Dofs] = W[Dofs];
                                        W_old[Dofs + 1] = W[Dofs + 1];
                                        CumH = CumH / (decimal)Length;
                                        CumW = CumW / (decimal)Length;

                                        // carbonatation----------------------------------------------- 03.10.2023 on ne prend pas en consid�ration le multicouche dans la carbonatation
                                        GRHe = GRHe + GHextern * (DeltaT / (3600 * 24));
                                        if (BDlibre == false)
                                            DRHe = DRHe + DHextern * (DeltaT / (3600 * 24));
                                        CumTemps = CumTemps + DeltaT / (3600 * 24);
                                        if (BDlibre == true)
                                        {
                                            Dxc = 0d;
                                        }
                                        else
                                        {
                                            Dxc = 2.8d *
                                                  Math.Pow(
                                                      (double)(RoC[Boucle2] / RoW) *
                                                      ((EC[Boucle2] - 0.3d) /
                                                       (double)(1f + RoC[Boucle2] / RoW * EC[Boucle2])) *
                                                      (double)(1f - DRHe / CumTemps), 2d) * CoefCa[Boucle2];
                                            Dxc = Math.Pow(
                                                (double)((1f + RoC[Boucle2] / RoW * EC[Boucle2] +
                                                          RoC[Boucle2] * qGran[Boucle2] /
                                                          (RoA[Boucle2] * ciment[Boucle2])) * DyCO2 * (float)Tijd) *
                                                Dxc * 24d * 3600d / 100d, 0.5d) / 28d;
                                        }

                                        Gxc = 2.8d *
                                              Math.Pow(
                                                  (double)(RoC[Boucle2] / RoW) *
                                                  ((EC[Boucle2] - 0.3d) /
                                                   (double)(1f + RoC[Boucle2] / RoW * EC[Boucle2])) *
                                                  (double)(1f - GRHe / CumTemps), 2d) * CoefCa[Boucle2];
                                        Gxc = Math.Pow(
                                            (double)((1f + RoC[Boucle2] / RoW * EC[Boucle2] + RoC[Boucle2] *
                                                         qGran[Boucle2] / (RoA[Boucle2] * ciment[Boucle2])) * GyCO2 *
                                                     (float)Tijd) * Gxc * 24d * 3600d / 100d, 0.5d) / 28d;
                                        if (Gxc < Gxcold)
                                            Gxc = Gxcold;
                                        if (Dxc < Dxcold)
                                            Dxc = Dxcold;
                                        Gxcold = Gxc;
                                        Dxcold = Dxc;
                                        // Gxc = Gxc * 1000
                                        // Dxc = Dxc * 1000
                                        var loopTo48 = (long)Dofs;
                                        for (j = 1; j <= loopTo48; j++)
                                        {
                                            if ((double)PosProf[(int)j] <= 1.12d + Gxc)
                                            {
                                                GPH = (float)(pPH * (mPh / pPH + (1d - mPh / pPH) /
                                                    (1d + Math.Pow(
                                                        (1d - ((double)PosProf[(int)j] - Gxc + 2.88d) / 4d) /
                                                        (1d - 0.5d), 4d))));
                                            }
                                            else
                                            {
                                                GPH = (float)pPH;
                                            }

                                            if (BDlibre == true)
                                            {
                                                DPH = (float)pPH;
                                            }
                                            else if ((double)(Length - (float)PosProf[(int)j]) <= 1.12d + Dxc)
                                            {
                                                DPH = (float)(pPH * (mPh / pPH + (1d - mPh / pPH) /
                                                    (1d + Math.Pow(
                                                        (1d - ((double)(Length - (float)PosProf[(int)j]) - Dxc +
                                                               2.88d) / 4d) / (1d - 0.5d), 4d))));
                                            }
                                            else
                                            {
                                                DPH = (float)pPH;
                                            }

                                            Ph[(int)j] = (decimal)GPH;
                                            if (GPH > DPH)
                                                Ph[(int)j] = (decimal)DPH;
                                            Gamma[(int)j] =
                                                (float)(Math.Exp(aOH * (1d - Math.Pow(10d, (double)Ph[(int)j] - pPH))) *
                                                    Math.Exp(EbG / R * (1d / (273.16d + (double)T_new[(int)j]) -
                                                                        (double)(1f / toG))) * ciment[Boucle2] *
                                                    Hydr[Boucle2] * faG / 1000d);
                                        }

                                        // next the chlorides ------------------------------------------
                                        // first the convection part (flow)
                                        // water flow
                                        testOld = 100m;
                                        var loopTo49 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo49; j++)
                                            Speed[(int)j] = 0.0m; // initialisation
                                        if ((double)GHextern > 0.9999d | (double)DHextern > 0.9999d)
                                        {
                                            Gctj = Dofs;
                                            var loopTo50 = (long)Dofs;
                                            for (j = 1L; j <= loopTo50; j++)
                                            {
                                                if (H_new[(int)(j + 1L)] >= H_new[(int)j] & Gctj == Dofs)
                                                    Gctj = j;
                                                if (PosProf[(int)j] > PosProf[(int)Gcptj] + 5m)
                                                {
                                                    Gctj = j - 1L;
                                                    break;
                                                }
                                            }

                                            if (Gctj < Gcptj)
                                                Gcptj = Gctj;
                                            if (BDlibre == false)
                                            {
                                                Dctj = 1L;
                                                for (j = Dofs; j >= 1L; j += -1)
                                                {
                                                    if (H_new[(int)(j - 1L)] >= H_new[(int)j] & Dcptj == 1L)
                                                        Dctj = j;
                                                    if (PosProf[(int)j] < PosProf[(int)Dctj] - 5m)
                                                    {
                                                        Dctj = j + 1L;
                                                        break;
                                                    }
                                                }

                                                if (Dctj > Dcptj)
                                                    Dcptj = Dctj;
                                            }
                                        }

                                        var loopTo51 = (long)Dofs;
                                        for (j = 0;
                                             j <= loopTo51;
                                             j++) // vitesse par la diffusion de vapeur d'eau seule
                                        {
                                            if (nCouches > 1) // 03.10.2023 d�but
                                            {
                                                var loopTo52 = (long)(nCouches - 1);
                                                for (k = 1L; k <= loopTo52; k++)
                                                {
                                                    if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                    {
                                                        decimal localMDCdiff3()
                                                        {
                                                            var tmp4 = CoefHr;
                                                            float argcoef4 = (float)tmp4[Boucle3];
                                                            var ret = TransChlorApi.Models.Functions.MDCdiff(
                                                                ref H_old[(int)j], ref argcoef4, ref PD[(int)k], ref Hc,
                                                                ref aa, ref ED[(int)k], ref ToHydr[(int)k],
                                                                ref T_old[(int)j]);
                                                            tmp4[Boucle3] = argcoef4;
                                                            return ret;
                                                        }

                                                        Ae[(int)j] = -localMDCdiff3() *
                                                            (H_old[(int)(j + 1)] - H_old[(int)j]) / 2.0m;
                                                        break;
                                                    }
                                                    else if (k == nCouches - 1)
                                                    {
                                                        decimal localMDCdiff4()
                                                        {
                                                            var tmp5 = CoefHr;
                                                            float argcoef5 = (float)tmp5[Boucle3];
                                                            var ret = TransChlorApi.Models.Functions.MDCdiff(
                                                                ref H_old[(int)j], ref argcoef5, ref PD[nCouches],
                                                                ref Hc, ref aa, ref ED[nCouches], ref ToHydr[nCouches],
                                                                ref T_old[(int)j]);
                                                            tmp5[Boucle3] = argcoef5;
                                                            return ret;
                                                        }

                                                        Ae[(int)j] = -localMDCdiff4() *
                                                            (H_old[(int)(j + 1)] - H_old[(int)j]) / 2.0m;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                decimal localMDCdiff5()
                                                {
                                                    var tmp6 = CoefHr;
                                                    float argcoef6 = (float)tmp6[Boucle3];
                                                    var ret = TransChlorApi.Models.Functions.MDCdiff(ref H_old[(int)j],
                                                        ref argcoef6, ref PD[Bou2], ref Hc, ref aa, ref ED[Bou2],
                                                        ref ToHydr[Bou2], ref T_old[(int)j]);
                                                    tmp6[Boucle3] = argcoef6;
                                                    return ret;
                                                }

                                                Ae[(int)j] = -localMDCdiff5() * (H_old[(int)(j + 1)] - H_old[(int)j]) /
                                                             2.0m;
                                            } // 03.10.2023 fin
                                        }

                                        var loopTo53 = (long)Dofs;
                                        for (j = 0; j <= loopTo53; j++)
                                        {
                                            if (Ae[(int)j] > 0m &
                                                (j < Gcptj | j > Dcptj)) // prise en compte de la capillarit�
                                            {
                                                var tmp = CoefCap;
                                                float argcoef = (float)tmp[Boucle4];
                                                bool argQuest = false;
                                                decimal argLength = (decimal)Length;
                                                decimal argLgLim = LgLim;
                                                Be[(int)j] = TransChlorApi.Models.Functions.MDCcap(ref GHextern,
                                                    ref DHextern, ref argcoef, ref DeltaT, ref tPrec, ref j, ref Tijd,
                                                    ref tijdOld, ref argQuest, ref ab, ref tc, ref DcapLeft, DcapRight,
                                                    ref BDlibre, ref PosProf[(int)j], ref argLength, ref argLgLim,
                                                    ref ImpHydr);
                                                tmp[Boucle4] = argcoef;
                                                Length = (float)argLength;
                                                LgLim = (int)Math.Round(argLgLim);
                                            }
                                            else if (Ae[(int)j] < 0m & (j < Gcptj | j > Dcptj))
                                            {
                                                decimal localMDCcap1()
                                                {
                                                    var tmp7 = CoefCap;
                                                    float argcoef7 = (float)tmp7[Boucle4];
                                                    bool argQuest1 = false;
                                                    decimal argLength1 = (decimal)Length;
                                                    decimal argLgLim1 = LgLim;
                                                    var ret = TransChlorApi.Models.Functions.MDCcap(ref GHextern,
                                                        ref DHextern, ref argcoef7, ref DeltaT, ref tPrec, ref j,
                                                        ref Tijd, ref tijdOld, ref argQuest1, ref ab, ref tc,
                                                        ref DcapLeft, DcapRight, ref BDlibre, ref PosProf[(int)j],
                                                        ref argLength1, ref argLgLim1, ref ImpHydr);
                                                    tmp7[Boucle4] = argcoef7;
                                                    Length = (float)argLength1;
                                                    LgLim = (int)Math.Round(argLgLim1);
                                                    return ret;
                                                }

                                                Be[(int)j] = -localMDCcap1();
                                            }
                                            else
                                            {
                                                Be[(int)j] = 0m;
                                            }

                                            if (Ae[(int)j] == 0m)
                                                Be[(int)j] = 0m;
                                        }

                                        Speed[0] = Ae[0] + Be[0];
                                        Speed[Dofs + 1] = Ae[Dofs] + Be[Dofs];
                                        var loopTo54 = (long)Dofs;
                                        for (j = 1; j <= loopTo54; j++) // assemblage, vitesse sur chaque noeud
                                            Speed[(int)j] = Ae[(int)(j - 1L)] + Ae[(int)j] + Be[(int)(j - 1L)] +
                                                            Be[(int)j];

                                        // use speed to compute the fictive position of the nodes
                                        PosProf[Dofs + 1] = (decimal)Length;
                                        var loopTo55 = (long)(Dofs + 1);
                                        for (j = 0; j <= loopTo55; j++)
                                        {
                                            H_old[(int)j] = H_new[(int)j];
                                            Speed[(int)j] = PosProf[(int)j] +
                                                            (decimal)Retard * Speed[(int)j] *
                                                            (decimal)
                                                            DeltaT; // d�placement des noeuds transformation de la vitesse en distance
                                            if ((double)Speed[(int)j] <= 0.0d)
                                                Speed[(int)j] = 0.0m; // les cl- ne peuvent pas sortir
                                            if ((float)Speed[(int)j] >= Length)
                                                Speed[(int)j] = (decimal)Length; // les cl- ne peuvent pas sortir
                                        }

                                        // compute the convective state
                                        // boundary
                                        Gamma[0] = Gamma[1];
                                        Gamma[Dofs + 1] = Gamma[Dofs];
                                        CT[0] = 0m;
                                        CT[Dofs + 1] = 0m;
                                        var loopTo56 = (long)(Dofs + 1);
                                        for (j = 0L; j <= loopTo56; j++)
                                        {
                                            Ae[(int)j] = 0m;
                                            Be[(int)j] = 0m;
                                        }

                                        var loopTo57 = (long)Dofs;
                                        for (j = 0L; j <= loopTo57; j++) // calcul des surfaces trap�zoidales
                                        {
                                            Ae[(int)j] = Ae[(int)j] + (C_trial[(int)j] * 3m + C_trial[(int)(j + 1L)]) *
                                                Le[(int)j] / 8m;
                                            Ae[(int)(j + 1L)] = (C_trial[(int)j] + C_trial[(int)(j + 1L)] * 3m) *
                                                Le[(int)j] / 8m;
                                        }

                                        Ae[1] = Ae[1] + Ae[0];
                                        Ae[Dofs] = Ae[Dofs] + Ae[Dofs + 1];
                                        SingVal = false;
                                        var loopTo58 = (long)(Dofs + 1);
                                        for (j = 0L; j <= loopTo58; j++) // calcul des r�actions d'appui sur les noeuds
                                        {
                                            Nbn = 0;
                                            Ltot = 0m;
                                            if (PosProf[(int)j] > Speed[(int)j])
                                            {
                                                var loopTo59 = (short)(Dofs + 1);
                                                for (jj = 0; jj <= loopTo59; jj++)
                                                {
                                                    if (PosProf[jj] >= Speed[(int)j] & jj != 0)
                                                    {
                                                        Nbn = (short)(Nbn + 1);
                                                        Ltot = Ltot + Le[jj - 1];
                                                        if (PosProf[jj] == PosProf[(int)j])
                                                            break;
                                                    }
                                                }

                                                Be[(int)(j - Nbn)] = Be[(int)(j - Nbn)] +
                                                                     (PosProf[(int)(j - Nbn + 1L)] - Speed[(int)j]) *
                                                                     Ae[(int)j] / Ltot;
                                                Be[(int)(j - Nbn + 1L)] = Be[(int)(j - Nbn + 1L)] +
                                                                          (Speed[(int)j] - PosProf[(int)(j - Nbn)]) *
                                                                          Ae[(int)j] / Ltot;
                                                var loopTo60 = (short)(j - 1L);
                                                for (jj = (short)(j - Nbn + 1L); jj <= loopTo60; jj++)
                                                {
                                                    Be[jj] = Be[jj] + Ae[(int)j] * Le[jj] / (Ltot * 2m);
                                                    Be[jj + 1] = Be[jj + 1] + Ae[(int)j] * Le[jj] / (Ltot * 2m);
                                                }
                                            }
                                            else if (PosProf[(int)j] < Speed[(int)j])
                                            {
                                                var loopTo61 = (short)(Dofs + 1);
                                                for (jj = 0; jj <= loopTo61; jj++)
                                                {
                                                    if (PosProf[jj] > PosProf[(int)j])
                                                    {
                                                        Nbn = (short)(Nbn + 1);
                                                        Ltot = Ltot + Le[jj - 1];
                                                        if (PosProf[jj] > Speed[(int)j])
                                                            break;
                                                    }
                                                }

                                                var loopTo62 = (short)(j + Nbn - 2L);
                                                for (jj = (short)j; jj <= loopTo62; jj++)
                                                {
                                                    Be[jj] = Be[jj] + Ae[(int)j] * Le[jj] / (Ltot * 2m);
                                                    Be[jj + 1] = Be[jj + 1] + Ae[(int)j] * Le[jj] / (Ltot * 2m);
                                                }

                                                Be[(int)(j + Nbn - 1L)] = Be[(int)(j + Nbn - 1L)] +
                                                                          (PosProf[(int)(j + Nbn)] - Speed[(int)j]) *
                                                                          Ae[(int)j] / Ltot;
                                                Be[(int)(j + Nbn)] = Be[(int)(j + Nbn)] +
                                                                     (Speed[(int)j] - PosProf[(int)(j + Nbn - 1L)]) *
                                                                     Ae[(int)j] / Ltot;
                                            }
                                            else
                                            {
                                                Be[(int)j] = Be[(int)j] + Ae[(int)j];
                                            }

                                            if (j != 0L)
                                            {
                                                if (Be[(int)(j - 1L)] != Be[(int)j] & SingVal == false)
                                                    SingVal = true;
                                            }
                                        }

                                        // next the diffusion part
                                        if (SingVal == false)
                                        {
                                            Nbn = 2;
                                        }
                                        else
                                        {
                                            Nbn = 1;
                                        }

                                        for (jj = Nbn; jj <= 2; jj++)
                                        {
                                            Again2: ;
                                            if (jj == 1)
                                            {
                                                DB = 0;
                                                CB = 1;
                                            }

                                            if (jj == 2)
                                            {
                                                DB = 1;
                                                CB = 0;
                                            }

                                            var loopTo63 = (long)(Dofs + 1);
                                            for (j = 0; j <= loopTo63; j++) // initialisation
                                            {
                                                LHS[1, (int)j] = 0.0m;
                                                LHS[2, (int)j] = 0.0m;
                                                RHS[(int)j] = 0.0m;
                                            }

                                            var loopTo64 = (long)(Dofs - 1);
                                            for (j = 1; j <= loopTo64; j++)
                                            {
                                                Ha = (W[(int)j] + W[(int)(j + 1)]) / 2000.0m;
                                                if ((double)C_trial[(int)j] < 0.01d)
                                                {
                                                    Mcap = Ha + (decimal)(Gamma[(int)j] * Math.Pow(0.01d, -0.621d));
                                                }
                                                else
                                                {
                                                    Mcap = Ha + (decimal)(Gamma[(int)j] *
                                                                          Math.Pow((double)C_trial[(int)j], -0.621d));
                                                }

                                                f1 = Mcap * Le[(int)j] / 3.0m;
                                                if (nCouches > 1) // 03.10.2023 d�but
                                                {
                                                    var loopTo65 = (long)(nCouches - 1);
                                                    for (k = 1L; k <= loopTo65; k++)
                                                    {
                                                        if ((double)PosProf[(int)j] < EpCouches[(int)k])
                                                        {
                                                            decimal localMDCl()
                                                            {
                                                                var tmp8 = CoefCl;
                                                                float argcoef8 = (float)tmp8[Boucle5];
                                                                var ret = TransChlorApi.Models.Functions.MDCl(
                                                                    ref Dcl[(int)k], ref Ecl[(int)k], ref ToCl[(int)k],
                                                                    ref T_old[(int)j], ref argcoef8);
                                                                tmp8[Boucle5] = argcoef8;
                                                                return ret;
                                                            }

                                                            f3 = f2 * Ha * localMDCl() / Le[(int)j];
                                                            break;
                                                        }
                                                        else if (k == nCouches - 1)
                                                        {
                                                            decimal localMDCl1()
                                                            {
                                                                var tmp9 = CoefCl;
                                                                float argcoef9 = (float)tmp9[Boucle5];
                                                                var ret = TransChlorApi.Models.Functions.MDCl(
                                                                    ref Dcl[nCouches], ref Ecl[nCouches],
                                                                    ref ToCl[nCouches], ref T_old[(int)j],
                                                                    ref argcoef9);
                                                                tmp9[Boucle5] = argcoef9;
                                                                return ret;
                                                            }

                                                            f3 = f2 * Ha * localMDCl1() / Le[(int)j];
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    decimal localMDCl2()
                                                    {
                                                        var tmp10 = CoefCl;
                                                        float argcoef10 = (float)tmp10[Boucle5];
                                                        var ret = TransChlorApi.Models.Functions.MDCl(ref Dcl[Bou2],
                                                            ref Ecl[Bou2], ref ToCl[Bou2], ref T_old[(int)j],
                                                            ref argcoef10);
                                                        tmp10[Boucle5] = argcoef10;
                                                        return ret;
                                                    }

                                                    f3 = f2 * Ha * localMDCl2() / Le[(int)j];
                                                }

                                                LHS[1, (int)j] = LHS[1, (int)j] + (f1 + f3) * DB + 3m * Le[(int)j] * CB;
                                                LHS[2, (int)j] = LHS[2, (int)j] + (f1 / 2.0m - f3) * DB +
                                                                 Le[(int)j] * CB;
                                                LHS[1, (int)(j + 1)] = LHS[1, (int)(j + 1)] + (f1 + f3) * DB +
                                                                       3m * Le[(int)j] * CB;
                                                C11 = f1 - f3;
                                                C12 = f1 / 2.0m + f3;
                                                RHS[(int)j] = RHS[(int)j] +
                                                              (C11 * C_old[(int)j] + C12 * C_old[(int)(j + 1)]) * DB +
                                                              8m * Be[(int)j] * CB;
                                                RHS[(int)(j + 1)] = RHS[(int)(j + 1)] +
                                                                    (C12 * C_old[(int)j] + C11 * C_old[(int)(j + 1)]) *
                                                                    DB;
                                            }

                                            // IMPOSE boundary condition
                                            if (W[0] == 0m)
                                                W[0] = W[1];
                                            GCboundary = (decimal)(GCFextern * 1000f / (float)W[0]);
                                            if (BDlibre == false)
                                            {
                                                if (W[Dofs + 1] == 0m)
                                                    W[Dofs + 1] = W[Dofs];
                                                DCboundary = (decimal)(DCFextern * 1000f / (float)W[Dofs + 1]);
                                            }

                                            RHS[1] = GCboundary;
                                            LHS[1, 1] = 1.0m;
                                            RHS[2] = RHS[2] - LHS[2, 1] * RHS[1];
                                            LHS[2, 1] = 0.0m;

                                            if (BDlibre == false)
                                            {
                                                RHS[Dofs] = DCboundary;
                                                LHS[1, Dofs] = 1.0m;
                                                RHS[Dofs - 1] = RHS[Dofs - 1] - LHS[2, Dofs - 1] * RHS[Dofs];
                                                LHS[2, Dofs - 1] = 0.0m;
                                            }

                                            // solve system of equations
                                            var loopTo66 = (long)Dofs;
                                            for (j = 2; j <= loopTo66; j++)
                                            {
                                                LHS[1, (int)j] = LHS[1, (int)j] - LHS[2, (int)(j - 1)] *
                                                    LHS[2, (int)(j - 1)] / LHS[1, (int)(j - 1)];
                                                RHS[(int)j] = RHS[(int)j] - RHS[(int)(j - 1)] * LHS[2, (int)(j - 1)] /
                                                    LHS[1, (int)(j - 1)];
                                            }

                                            C_trial[Dofs] = RHS[Dofs] / LHS[1, Dofs];
                                            for (j = Dofs - 1; j >= 1; j += -1)
                                                C_trial[(int)j] =
                                                    (RHS[(int)j] - LHS[2, (int)j] * C_trial[(int)(j + 1)]) /
                                                    LHS[1, (int)j];
                                            var loopTo67 = (long)(Dofs + 1);
                                            for (j = 0L; j <= loopTo67; j++)
                                            {
                                                if (C_trial[(int)j] < 0m)
                                                    C_trial[(int)j] = 0m;
                                                if (C_trial[(int)j] > Msel * 1000m / W[(int)j])
                                                    C_trial[(int)j] = Msel * 1000m / W[(int)j];
                                            }

                                            // check on convergence
                                            test = 0m;
                                            var loopTo68 = (long)(Dofs + 1);
                                            for (j = 0; j <= loopTo68; j++)
                                            {
                                                if (Math.Abs(C_trial[(int)j] - C_new[(int)j]) > test)
                                                    test = Math.Abs(C_trial[(int)j] - C_new[(int)j]);
                                                C_trial[(int)j] = (decimal)(0.6d * (double)C_new[(int)j] +
                                                                            0.4d * (double)C_trial[(int)j]);
                                                C_new[(int)j] = C_trial[(int)j];
                                            }

                                            if (testOld <= test)
                                                test = (decimal)(testing2 / 10f); // en cas de non convergence
                                            testOld = test;
                                            if ((float)test > testing2)
                                                goto Again2;
                                            testOld = 100m;
                                            if (jj ==
                                                1) // calcul des ions chlorures li�es apr�s l'entra�nement des ions par l'eau
                                            {
                                                var loopTo69 = (long)(Dofs + 1);
                                                for (j = 0; j <= loopTo69; j++)
                                                {
                                                    if (C_trial[(int)j] < 0m)
                                                        C_trial[(int)j] =
                                                            (C_trial[(int)(j - 1L)] + C_trial[(int)(j + 1L)]) /
                                                            2m; // correction pour des probl�mes num�riques
                                                    if (C_trial[(int)j] < 0m)
                                                        C_trial[(int)j] = 0m;
                                                    CBo[(int)j] = (decimal)(Gamma[(int)j] *
                                                                            Math.Pow((double)C_trial[(int)j], 0.379d));
                                                    if (CT[(int)j] == 0m)
                                                    {
                                                        CT[(int)j] = C_trial[(int)j] * W[(int)j] / 1000m;
                                                        C_trial[(int)j] = CT[(int)j] - CBo[(int)j];
                                                        if (C_trial[(int)j] < 0m)
                                                            C_trial[(int)j] = 0m;
                                                    }
                                                    else
                                                    {
                                                        CT[(int)j] =
                                                            (decimal)((double)(C_trial[(int)j] * W[(int)j] / 1000m) +
                                                                      Gamma[(int)j] * Math.Pow((double)C_trial[(int)j],
                                                                          0.379d));
                                                    }

                                                    C_new[(int)j] = C_trial[(int)j];
                                                    C_old[(int)j] = C_trial[(int)j];
                                                }
                                            }
                                        }

                                        // update variables
                                        SingVal = false; // traitement des valeurs singuli�res
                                        var loopTo70 = (long)Dofs;
                                        for (j = 1; j <= loopTo70; j++)
                                        {
                                            if (C_new[(int)j] < 0m)
                                                C_new[(int)j] = (C_new[(int)(j - 1L)] + C_new[(int)(j + 1L)]) / 2m;
                                            if (C_new[(int)j] > Msel * 1000m / W[(int)j])
                                                C_new[(int)j] = Msel * 1000m / W[(int)j];
                                            if (C_new[(int)j] < 0m)
                                                C_new[(int)j] = 0m;
                                            C_trial[(int)j] = C_new[(int)j];
                                            CT[(int)j] = (decimal)((double)(C_new[(int)j] * W[(int)j] / 1000m) +
                                                                   Gamma[(int)j] * Math.Pow((double)C_new[(int)j],
                                                                       0.379d)); // kg/m3 de b�ton 
                                            if (CTmax < (double)CT[(int)j])
                                                CTmax = (double)CT[(int)j];
                                            if (CLmax < (double)(C_new[(int)j] * W[(int)j]) / 1000.0d)
                                                CLmax = (double)(C_new[(int)j] * W[(int)j]) / 1000.0d;
                                        }

                                        if (CTmold < CTmax)
                                        {
                                            CTmax = (int)Math.Round(CTmax) + 1;
                                            CTmold = (int)Math.Round(CTmax);
                                        }

                                        if (CLmold < CLmax)
                                        {
                                            CLmax = (int)Math.Round(CLmax) + 1;
                                            CLmold = (int)Math.Round(CLmax);
                                        }

                                        if (Ssym == true)
                                        {
                                            var loopTo71 = (long)(short)Math.Round((Dofs + 1) / 2d);
                                            for (j = 0; j <= loopTo71; j++)
                                            {
                                                C_new[(int)j] = (C_new[(int)j] + C_new[(int)(Dofs + 1 - j)]) / 2m;
                                                C_new[(int)(Dofs + 1 - j)] = C_new[(int)j];
                                                C_old[(int)j] = C_new[(int)j];
                                                C_old[(int)(Dofs + 1 - j)] = C_old[(int)j];
                                            }
                                        }

                                        if (BDlibre == true) // probl�me num�rique sur le deuxi�me bord
                                        {
                                            Ctest = Msel * 1000m / W[1];
                                            for (j = Dofs; j >= 1; j += -1)
                                            {
                                                if (Math.Abs(C_new[(int)j] - C_new[(int)(j - 1L)]) >= Ctest / 4m)
                                                {
                                                    Ctest = Math.Abs(C_new[(int)j] - C_new[(int)(j - 1L)]);
                                                    C_new[(int)j] = C_new[(int)(j + 1L)];
                                                    CT[(int)j] = (decimal)((double)(C_new[(int)j] * W[(int)j] / 1000m) +
                                                                           Gamma[(int)j] *
                                                                           Math.Pow((double)C_new[(int)j],
                                                                               0.379d)); // kg/m3 de b�ton
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        // '______________________ 'pour le programme 

                                        // If Tijd = 0.25 Then '1 an ou 365 jours
                                        // Hteller = Hteller + CDbl(Hsauv)
                                        // Register(nFic1, Tijd, Dofs, H_new)
                                        // PrintLine(nFic1, " ")
                                        // Wteller = Wteller + CDbl(Wsauv)
                                        // Register(nFic2, Tijd, Dofs, W)
                                        // PrintLine(nFic2, CumW, ", ")
                                        // CTteller = CTteller + CDbl(CTsauv)
                                        // Register(nFic3, Tijd, Dofs, CT)
                                        // PrintLine(nFic3, " ")
                                        // CLteller = CLteller + CDbl(CLsauv)
                                        // Regist01(nFic4, Tijd, Dofs, C_new, W)
                                        // PrintLine(nFic4, " ")
                                        // Tteller = Tteller + CDbl(Tsauv)
                                        // Register(nFic5, Tijd, Dofs, T_new)
                                        // PrintLine(nFic5, " ")
                                        // Carbteller = Carbteller + CDbl(Carbsauv)
                                        // Print(CInt(nFic6), Tijd / 365, ",", Tijd, ",", Gxc, ",", Dxc, ",", TAB)
                                        // PrintLine(nFic6, " ")
                                        // Register(nFic7, Tijd, Dofs, Ph)
                                        // PrintLine(nFic7, " ")
                                        // End If
                                        // '______________________

                                        t1 = 0.00001f;
                                        t2 = (float)Tijd + DeltaT / 3600f / 24f;
                                        if (Cond01 > 0)
                                            goto Again3; // delta T interm�diaire

                                        cancellationToken.ThrowIfCancellationRequested();
                                        if (i >= (long)Math.Round(affiche)) // 1 mois ou 30 jours
                                        {
                                            affiche = affiche + taff;
                                            // Update the pictures
                                            int argCTmax1 = (int)Math.Round(CTmax);
                                            int argCLmax1 = (int)Math.Round(CLmax);
                                            float argGxc1 = (float)Gxc;
                                            float argDxc1 = (float)Dxc;
                                            //dessin graphiques
                                            // frm.design(ref Wsat, ref H_old, ref W, ref T_old, ref C_new, ref CT, ref TempMin, ref TempMax, ref Length, ref PosProf, ref hMin, ref hEcart, ref wMin, ref wEcart, ref CTmin, ref CTecart, ref argCTmax1, ref CLmin, ref CLecart, ref argCLmax1, ref Tecart, ref Dofs, ref Tijd, ref argGxc1, ref argDxc1, ref Ph);
                                            CTmax = argCTmax1;
                                            CLmax = argCLmax1;
                                            Gxc = (double)argGxc1;
                                            Dxc = (double)argDxc1;

                                            //ajout
                                            float argpara = 0f;
                                            var parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = H_new,
                                                Para = argpara,
                                                type = "moisture_potential"
                                            };
                                            Register(parameters,"30days");

                                            float argpara1 = 0f;
                                            parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = W,
                                                Para = argpara1,
                                                type = "moisture_content"
                                            };
                                            Register(parameters,"30days");

                                            float argpara2 = 0f;
                                            parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = CT,
                                                Para = argpara2,
                                                type = "total_chloride"
                                            };
                                            Register(parameters,"30days");

                                            short argPara = (short)((short)(NprobHr + NprobCap) + NprobCl);
                                            Regist01Async(Tijd, Dofs, C_new, W, ciment, argPara, nCouches, EpCouches, Boucle2, "free_chloride","30days");

                                            if (NprobHr == 1 & NprobCap == 1 & NprobCl == 1)
                                            {
                                                float argpara3 = 0f;
                                                parameters = new RegisterParameters
                                                {
                                                    Tijd = Tijd,
                                                    Dofs = Dofs,
                                                    H_new = T_new,
                                                    Para = argpara3,
                                                    type = "temperature_potential"
                                                };

                                                Register(parameters,"30days");
                                            }
                                            else // si approche probabiliste
                                            {
                                                float argpara4 = 273.16f;
                                                parameters = new RegisterParameters
                                                {
                                                    Tijd = Tijd,
                                                    Dofs = Dofs,
                                                    H_new = T_new,
                                                    Para = argpara4,
                                                    type = "temperature_potential"
                                                };

                                                Register(parameters,"30days");
                                            }
                                            
                                            float argpara5 = 0f;
                                            parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = Ph,
                                                Para = argpara5,
                                                type = "ph"
                                            };
                                            
                                            Register(parameters,"30days");
                                        }
                                    
                                        //end of the 30 days
                                        
                                        if (i >= (long)Math.Round(Hteller)) // 1 an ou 365 jours
                                        {
                                            Hteller = Hteller + Hsauv;
                                            float argpara = 0f;
                                            var parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = H_new,
                                                Para = argpara,
                                                type = "moisture_potential"
                                            };
                                            
                                            Register(parameters,"365days");
                                            // HRCUMUL(cpt2, cpt1) = CumH
                                            // cpt2 = cpt2 + 1
                                        }
                                        if (i >= (long)Math.Round(Wteller)) // 1 an ou 365 jours
                                        {
                                            Wteller = Wteller + Wsauv;
                                            float argpara1 = 0f;
                                            var parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = W,
                                                Para = argpara1,
                                                type = "moisture_content"
                                            };
                                            
                                            Register(parameters,"365days");
                                            // Register(ref nFic2, ref Tijd, ref Dofs, ref W, ref argpara1);
                                        }
                                        if (i >= (long)Math.Round(CTteller)) // 1 an ou 365 jours
                                        {
                                            CTteller = CTteller + CTsauv;
                                            float argpara2 = 0f;
                                            var parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = CT,
                                                Para = argpara2,
                                                type = "total_chloride"
                                            };
                                            
                                            Register(parameters,"365days");
                                            // Register(ref nFic3, ref Tijd, ref Dofs, ref CT, ref argpara2);
                                            // PrintLine(nFic8, CumClT)        'prov
                                        }
                                        if (i >= (long)Math.Round(CLteller)) // 1 an ou 365 jours
                                        {
                                            CLteller = CLteller + CLsauv;
                                            short argPara = (short)((short)(NprobHr + NprobCap) + NprobCl);
                                            
                                            Regist01Async(Tijd, Dofs, C_new, W, ciment, argPara, nCouches, EpCouches, Boucle2, "free_chloride", "365days");
                                            // Regist01(ref nFic4, ref Tijd, ref Dofs, ref C_new, ref W, ref ciment, ref argPara, ref nCouches, ref EpCouches, ref Boucle2);   // 03.10.2023 changement ciment(Boucle2) par ciment
                                        }
                                        if (i >= (long)Math.Round(Tteller)) // 1 an ou 365 jours
                                        {
                                            Tteller = Tteller + Tsauv;
                                            if (NprobHr == 1 & NprobCap == 1 & NprobCl == 1)
                                            {
                                                float argpara3 = 0f;
                                                var parameters = new RegisterParameters
                                                {
                                                    Tijd = Tijd,
                                                    Dofs = Dofs,
                                                    H_new = T_new,
                                                    Para = argpara3,
                                                    type = "temperature_potential"
                                                };
                                            
                                                Register(parameters,"365days");
                                                // Register(ref nFic5, ref Tijd, ref Dofs, ref T_new, ref argpara3);
                                            }
                                            else        // si approche probabiliste
                                            {
                                                float argpara4 = 273.16f;
                                                var parameters = new RegisterParameters
                                                {
                                                    Tijd = Tijd,
                                                    Dofs = Dofs,
                                                    H_new = T_new,
                                                    Para = argpara4,
                                                    type="temperature_potential"
                                                };
                                            
                                                Register(parameters,"365days");
                                                // Register(ref nFic5, ref Tijd, ref Dofs, ref T_new, ref argpara4);
                                            }
                                        }
                                        if (i >= (long)Math.Round(Carbteller)) // 1 an ou 365 jours
                                        {
                                            Carbteller = Carbteller + Carbsauv;
                                            float argpara5 = 0f;
                                            var parameters = new RegisterParameters
                                            {
                                                Tijd = Tijd,
                                                Dofs = Dofs,
                                                H_new = Ph,
                                                Para = argpara5,
                                                type = "ph"
                                            };
                                            
                                            Register(parameters,"365days");
                                            // Register(ref nFic7, ref Tijd, ref Dofs, ref Ph, ref argpara5);
                                        }
                                        // Application.DoEvents(); 
                                        if (Tijd >= (decimal)TimeMax)
                                            break;
                                        iG = iG + 1L;
                                        iD = iD + 1L;
                                        i_day = i_day + 1L;
                                    }
                                    // FileClose(CInt(nFic8))      'prov
                                }
                            }
                        }
                    }
                }

            BreakBoucle1:
                ;

            }

            // programmation pour obtenir des fichiers de comparaison (provisoire)
            // For j = 0 To cpt2 - 1
            // If j = 0 Then
            // For i = 1 To cpt1
            // Print(CInt(nFile), i, ",", TAB)
            // Next
            // PrintLine(CInt(nFile), "")
            // End If
            // For i = 1 To cpt1
            // Print(CInt(nFile), HRCUMUL(j, i), ",", TAB)
            // Next
            // PrintLine(CInt(nFile), "")
            // Next
            // FileClose(CInt(nFile))

            // Interaction.MsgBox("Fin du calcul", MsgBoxStyle.OkOnly & MsgBoxStyle.Information, "Fin");
            Console.WriteLine("Fin du calcul !");

            // frm.ModifyCommand1(false);
            // frm02.Command1..Enabled = False

        }

        // Enregistrement des donn�es dans les fichiers d'output
        private async Task Register(RegisterParameters parameters, string registerType)
        {
            short j;
            
            var values = new List<double>();
            
            
            
            for (j = 1; j <= parameters.Dofs; j++)
            {
                values.Add((double)((decimal)parameters.H_new[j] + (decimal)parameters.Para)); // Conversion de para en decimal
            }
            
            var result = new ComputationResult
            {
                Time = (float)(parameters.Tijd),
                Values = values,
                Type = parameters.type,
                ComputationId = computationId,
            };
            
            if (registerType == "30days")
            {
                await httpClient.PostAsJsonAsync("computations-actual-results", result);
            }
            else if (registerType == "365days")
            {
                await httpClient.PostAsJsonAsync("computations-results", result);
                await httpClient.PostAsJsonAsync("computations-actual-results", result);
            }
            else
            {
                throw new Exception("You have to choose between 30 days or 365 days case");
            }
        }


        private async Task Regist01Async(
            decimal Tijd,
            short Dofs,
            decimal[] CL_new,
            decimal[] W,
            float[] Ciment,
            short Para,
            int nCouches,
            double[] EpCouches,
            short Boucle2,
            string type,
            string registerType)
        {
            short j, k;
            float Cim;
            
            var values = new List<double>();

            for (j = 1; j <= Dofs; j++)
            {
                if (nCouches > 1)
                {
                    for (k = 1; k < nCouches; k++)
                    {
                        if ((double)PosProf[j] < EpCouches[k])
                        {
                            Cim = Ciment[k];
                            goto CalculeValeur;
                        }
                    }
                    Cim = Ciment[nCouches];
                }
                else
                {
                    Cim = Ciment[Boucle2];
                }

                CalculeValeur:
                decimal valeur;

                if (Para == 3)
                {
                    valeur = CL_new[j] * W[j] / 1000m;
                }
                else
                {
                    valeur = (decimal)((float)(CL_new[j] * W[j]) / (10f * Cim));
                }
                
                values.Add((double)valeur);
            }

            var result = new ComputationResult
            {
                Time = (float)Tijd,
                Values = values,
                Type = type,
                ComputationId = computationId,
            };

            if (registerType == "30days")
            {
                await httpClient.PostAsJsonAsync("computations-actual-results", result);
            }
            else if (registerType == "365days")
            {
                await httpClient.PostAsJsonAsync("computations-results", result);
                await httpClient.PostAsJsonAsync("computations-actual-results", result);
            }
            else
            {
                throw new Exception("You have to choose between 30 days or 365 days case");
            }
            
        }

    }
}