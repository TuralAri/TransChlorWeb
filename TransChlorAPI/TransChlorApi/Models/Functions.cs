using System;

namespace TransChlorApi.Models
{
    static class Functions
    {

        // calcul du coefficient de diffusion thermique
        public static decimal MT(ref float qGran, ref float capCal, ref decimal W, ref float ciment, ref float Hg, ref decimal T_old)
        {
            decimal MTRet = default;
            decimal cT;
            const float cW = 4.2f;
            const float cZ = 0.84f;
            decimal Temper = 0m;
            var TT = new decimal[5];
            var Para = new decimal[5];
            var A = new double[5, 5];
            decimal tempo;
            decimal Lambda;
            short i;
            short k;
            // Dim Wc As Decimal
            // Wc = W
            // If Wc < 0 Then Wc = 0
            // If Wc > 500 Then Wc = 100
            A[0, 0] = 1.89890482236446d;
            A[1, 0] = 0.0150204938034272d;
            A[2, 0] = -0.00199503283634373d;
            A[3, 0] = 0.000933636457388569d;
            A[4, 0] = -0.000036168677689976d;
            A[0, 1] = 0.00336784223942822d;
            A[1, 1] = -0.00108514866715481d;
            A[2, 1] = 0.00717045610756123d;
            A[3, 1] = -0.000838746806026253d;
            A[4, 1] = 0.0000225795479813709d;
            A[0, 2] = -0.000177541199440964d;
            A[1, 2] = 0.0000048925253459615d;
            A[2, 2] = -0.000266164523769935d;
            A[3, 2] = 0.0000309974453424371d;
            A[4, 2] = -0.000000808707665874939d;
            A[0, 3] = 0.00000319492127255241d;
            A[1, 3] = 0.000000313219805503464d;
            A[2, 3] = 0.00000363355856819264d;
            A[3, 3] = -0.000000418290254241918d;
            A[4, 3] = 0.0000000104601101566469d;
            A[0, 4] = -0.0000000186580757693275d;
            A[1, 4] = -0.00000000266810509407622d;
            A[2, 4] = -0.0000000174502096308597d;
            A[3, 4] = 0.00000000197825353025406d;
            A[4, 4] = -0.0000000000472449653832042d;

            cT = (decimal)qGran * (decimal)capCal + W * (decimal)cW + (decimal)ciment * (decimal)cZ - 0.2m * (decimal)ciment * (decimal)Hg * (decimal)cW;
            if (T_old < 0m)
            {
                Temper = 0m;
            }
            else
            {
                Temper = T_old;
            }
            for (i = 0; i <= 4; i++)
                TT[i] = (decimal)Math.Pow((double)Temper, i);
            for (i = 0; i <= 4; i++)
            {
                tempo = 0m;
                for (k = 0; k <= 4; k++)
                    tempo = tempo + (decimal)A[i, k] * TT[k];
                Para[i] = tempo;
            }
            Lambda = 0m;
            for (i = 0; i <= 4; i++)
            {
                TT[i] = (decimal)Math.Pow((double)(W / 10m), i);
                Lambda = Lambda + Para[i] * TT[i];
            }
            MTRet = 1000m * Lambda / cT;
            return MTRet;
        }

        // calcul du coefficient de diffusion hydrique capillarit�
        public static decimal MDCcap(ref float Bord1, ref float Bord2, ref float coef, ref float deltaT, ref float tPrec, ref long j, ref decimal Tijd, ref decimal TijdOld, ref bool Quest, ref float aa, ref float Hc, ref decimal Dcapleft, decimal Dcapright, ref bool BDlibre, ref decimal PosProf, ref decimal Length, ref decimal LgLim, ref bool ImpHydr)
        {
            decimal MDCcapRet = default;
            float Factor;
            // MDC = moisture diffusion coefficient
            // Classical formulea of Bazant

            if (aa == 0f & Hc == 0f)
            {
                MDCcapRet = 0m;
            }
            else
            {
                if (ImpHydr == false)
                {
                    if (BDlibre == false)
                    {
                        if (PosProf <= LgLim)
                        {
                            MDCcapRet = Dcapleft;
                        }
                        else if (PosProf >= Length - LgLim)
                        {
                            MDCcapRet = Dcapright;
                        }
                        else
                        {
                            MDCcapRet = ((Dcapright - Dcapleft) * PosProf + Dcapleft * Length - LgLim * (Dcapright + Dcapleft)) / (Length - 2m * LgLim);
                        }
                    }
                    else
                    {
                        MDCcapRet = Dcapleft;
                    }
                }
                else
                {
                    MDCcapRet = 0.000129m;
                }
                if (Bord1 > 0.9999f | Bord2 > 0.9999f) // si pluie
                {
                    if (j == 0L & Tijd != TijdOld & Quest == true)
                        tPrec = tPrec + deltaT / (3600 * 24);
                    if (tPrec > 10f)
                    {
                        Factor = (float)(decimal)Math.Pow((double)(1 / (1.0m - (decimal)Hc)), 4d);
                    }
                    else
                    {
                        Factor = (float)(decimal)Math.Pow((double)(tPrec / 10f / (float)(1.0m - (decimal)Hc)), 4d);
                    }
                    MDCcapRet = (decimal)((float)MDCcapRet * ((float)(decimal)aa + (float)(1.0m - (decimal)aa) / ((float)1.0m + Factor)));
                }
                else
                {
                    MDCcapRet = 0m;
                    if (j == 0L & Tijd != TijdOld & Quest == true)
                        tPrec = 0f;
                }
            }

            // prise en compte de la temp�rature
            MDCcapRet = MDCcapRet * (decimal)coef;
            TijdOld = Tijd;
            return MDCcapRet;
        }

        // calcul du coefficient de diffusion hydrique diffusion de vapeur d'eau
        public static decimal MDCdiff(ref decimal H, ref float coef, ref float PD, ref float Hc, ref float aa, ref float ED, ref float ToHydr, ref decimal Temperature)
        {
            decimal MDCdiffRet = default;
            decimal Factor;
            const float RR = 8.3144f;     // J/(�K.mol) constante des gaz parfaits

            if (H > 1.0m)
            {
                MDCdiffRet = (decimal)PD;
            }
            else if (H < 0.0m)
            {
                MDCdiffRet = (decimal)((float)(decimal)aa * PD);
            }
            else
            {
                Factor = (decimal)Math.Pow((double)((1.0m - H) / (1.0m - (decimal)Hc)), 4d);
                MDCdiffRet = (decimal)(PD * (float)((decimal)aa + (1.0m - (decimal)aa) / (1.0m + Factor)));
            }
            // prise en compte de la temp�rature
            MDCdiffRet = (decimal)((double)MDCdiffRet * Math.Exp((double)-ED * ((double)(1f / ToHydr) - 1d / ((double)Temperature + 273.16d)) / (double)RR));
            MDCdiffRet = MDCdiffRet * (decimal)coef;
            return MDCdiffRet;
        }

        // calcul du coefficient de diffusion des ions chlorures
        public static decimal MDCl(ref float Di, ref float Ecl, ref float ToCl, ref decimal Temperature, ref float coef)
        {
            decimal MDClRet = default;
            // Const RR As Single = 8.3144     'J/(�K.mol) constante des gaz parfaits
            MDClRet = (decimal)((double)Di * Math.Exp((double)(Ecl * ((float)Temperature - ToCl))));
            MDClRet = MDClRet * (decimal)coef;
            return MDClRet;
        }

        // courbe d'adsorption et de d�sorption
        public static decimal Water(decimal H, decimal Hol, ref decimal T, ref decimal Tijd, ref float tPro, ref float Vct, ref float Nct, ref float EsC, ref float Wsat, ref float Hydr, ref float ciment, ref bool Wol)
        {
            decimal WaterRet = default;
            decimal Time;
            decimal f1;
            decimal f2;
            decimal f3;
            decimal f4;
            decimal f5;
            decimal f6;
            decimal f7;
            decimal f8;
            decimal f9;
            decimal f10;
            decimal f11;
            decimal f12;
            decimal f13;
            var Para = new decimal[4, 4];
            var b = new decimal[4];
            var a = new decimal[4];
            decimal wcOld, wcNew, wNew;
            short i;
            short j;
            bool tst01 = false;
            float VH;
            float VHol;
            Time = Tijd + (decimal)tPro;
            VH = (float)((int)Math.Round(H * 1000m) / 1000d);
            VHol = (float)((int)Math.Round(Hol * 1000m) / 1000d);
            H = (decimal)((int)Math.Round(H * 100m) / 100d);
            Hol = (decimal)((int)Math.Round(Hol * 100m) / 100d);
            if (T < -100)
                T = -100;
            if (T > 100m)
                T = 100m;
            if ((double)Math.Abs(H - Hol) == 0.01d & (double)Math.Abs(VH - VHol) < 0.02d)
                Hol = H; // court-circuite les petites valeurs proche 0.05
            if (H == Hol)
            {
                if (Wol == true)
                {
                    H = (decimal)((double)H - 0.1d);
                }
                else
                {
                    H = (decimal)((double)H + 0.1d);
                }
                tst01 = true;
            }
            if (H <= Hol)
            {
                if (tst01 == true)
                {
                    H = (decimal)((double)H + 0.1d);
                    tst01 = false;
                }
                Wol = true;
                // On the desorption branch
                f1 = 0.125m;                                                          // c1
                f2 = (decimal)(0.173d - 0.431d * (double)((T - 20m) / 25m));                                // c2
                f3 = (decimal)(0.06d + 0.392d * (double)((T - 20m) / 25m));                                 // c3
                f4 = (decimal)(0.17d + (0.035d + 0.029d * ((double)EsC - 0.4d) / 0.15d) * (double)((T - 20m) / 25m));  // c4

                f5 = (decimal)(0.389d - (double)(26f * EsC / 15f) + 4.4d * Math.Pow((double)EsC, 2d) - 8d * Math.Pow((double)EsC, 3d) / 3d);        // we
                f6 = (decimal)(1d - 0.161d * (double)Hydr);                                               // ht

                Para[1, 1] = (decimal)Math.Pow((double)f6, 2d);             // para1
                Para[1, 2] = 1m - 2m * f6;         // para2
                Para[1, 3] = Para[1, 1] - f6;    // para3
                Para[2, 1] = -2 * f6;            // para4
                Para[2, 2] = -Para[2, 1];        // para5
                Para[2, 3] = 1m - Para[1, 1];     // para6
                Para[3, 1] = 1m;                  // para7
                Para[3, 2] = -1;                 // para8
                Para[3, 3] = f6 - 1m;             // para9

                b[1] = (decimal)(EsC - (float)f4 * Hydr);                          // b1
                b[2] = (decimal)(((double)(f1 + f2 * f6) + (double)f3 * Math.Pow((double)f6, 2d)) * (double)Hydr);      // b2
                b[3] = (decimal)((float)(f2 + 2m * f3 * f6) * Hydr);                // b3

                for (i = 1; i <= 3; i++)
                    a[i] = 0m;

                for (i = 1; i <= 3; i++)
                {
                    for (j = 1; j <= 3; j++)
                        a[i] = a[i] + Para[i, j] * b[j];
                }

                f7 = (decimal)(1d / Math.Pow((double)(1m - f6), 2d));
                for (i = 1; i <= 3; i++)
                    a[i] = a[i] * f7;         // a, b, c
                f8 = 0m;                                                  // A
                f9 = (decimal)((double)Hydr / 0.35d * ((double)-f1 / 0.35d + 0.35d * (double)f3));             // C
                f10 = (decimal)(((double)f2 + 0.7d * (double)f3) * (double)Hydr - 0.7d * (double)f9);                 // B
                f11 = (decimal)(((double)a[1] + (double)a[2] * 1.0d + (double)a[3] * Math.Pow(1.0d, 2d)) * (double)ciment);     // wm
                f11 = (decimal)(ciment * Wsat / (float)f11);

                switch (Hol)
                {
                    case var @case when @case <= 0.35m:
                        {
                            wcOld = (decimal)(((double)(f8 + f10 * Hol) + (double)f9 * Math.Pow((double)Hol, 2d)) * (double)f11);
                            break;
                        }
                    case var case1 when case1 < f6:
                        {
                            wcOld = (decimal)(((double)(f1 + f2 * Hol) + (double)f3 * Math.Pow((double)Hol, 2d)) * (double)Hydr * (double)f11);
                            break;
                        }

                    default:
                        {
                            wcOld = (decimal)(((double)(a[1] + a[2] * Hol) + (double)a[3] * Math.Pow((double)Hol, 2d)) * (double)f11);
                            break;
                        }
                }
                switch (H)
                {
                    case var case2 when case2 <= 0.35m:
                        {
                            wcNew = (decimal)(((double)(f8 + f10 * H) + (double)f9 * Math.Pow((double)H, 2d)) * (double)f11);
                            break;
                        }
                    case var case3 when case3 < f6:
                        {
                            wcNew = (decimal)(((double)(f1 + f2 * H) + (double)f3 * Math.Pow((double)H, 2d)) * (double)Hydr * (double)f11);
                            break;
                        }

                    default:
                        {
                            wcNew = (decimal)(((double)(a[1] + a[2] * H) + (double)a[3] * Math.Pow((double)H, 2d)) * (double)f11);
                            break;
                        }
                }

                if ((float)wcOld > Wsat)
                    wcOld = (decimal)Wsat;
                if ((float)wcNew > Wsat)
                    wcNew = (decimal)Wsat;
                if (wcOld < 0m)
                    wcOld = 0m;
                if (wcNew < 0m)
                    wcNew = 0m;

                WaterRet = wcNew;
            }

            else
            {
                if (tst01 == true)
                {
                    H = (decimal)((double)H - 0.1d);
                    tst01 = false;
                }
                Wol = false;
                // On the sorption branche
                if (Time < 5m)      // calcul de Vt(t) correstpondant � f2 et Nt(t) = f3
                {
                    f2 = 0.024m;
                    f3 = 5.5m;
                }
                else
                {
                    f2 = 0.068m - 0.22m / Time;
                    f3 = 2.5m + 15m / Time;
                }
                f2 = f2 * (decimal)Vct * (0.85m + 0.45m * (decimal)EsC) * 1.0m; // calcul de vm
                f3 = f3 * (0.33m + 2.2m * (decimal)EsC) * (decimal)Nct * 1.0m; // calcul de n
                f4 = (decimal)Math.Exp(855d / (273.16d + (double)T)); // calcul de C
                f5 = ((1m - 1m / f3) * f4 - 1m) / (f4 - 1m); // calcul de k
                f6 = (decimal)(ciment * (1f + EsC));
                f9 = f6 * f4 * f5 * f2 * 1.0m / ((1m - f5 * 1.0m) * (1m + (f4 - 1m) * f5 * 1.0m)); // calcul de wmax
                wcOld = f6 * f4 * f5 * f2 * Hol / ((1m - f5 * Hol) * (1m + (f4 - 1m) * f5 * Hol));
                wcNew = f6 * f4 * f5 * f2 * H / ((1m - f5 * H) * (1m + (f4 - 1m) * f5 * H));
                wcOld = (decimal)((float)wcOld * Wsat / (float)f9);
                wcNew = (decimal)((float)wcNew * Wsat / (float)f9);

                if ((float)wcOld > Wsat)
                    wcOld = (decimal)Wsat;
                if ((float)wcNew > Wsat)
                    wcNew = (decimal)Wsat;
                if (wcOld < 0m)
                    wcOld = 0m;
                if (wcNew < 0m)
                    wcNew = 0m;

                WaterRet = wcNew;

            }

            // If CDbl(H * 1000) <= CDbl(Hol * 1000) Then Water = Wol
            if ((float)WaterRet > Wsat)
                WaterRet = (decimal)Wsat;
            if (WaterRet < 0m)
                WaterRet = 0m;
            Hol = H;
            return WaterRet;
        }

        // calcul par it�ration le passage de cT � cf
        public static decimal Cfree(decimal cT, decimal gamma, decimal w)
        {
            decimal CfreeRet = default;
            float Deltacf;
            bool SensPos = true;
            decimal ItcT;
            decimal test = 10m;
            Deltacf = 1f;
            CfreeRet = (decimal)(0.0000001d - 1d);
            while ((double)Math.Abs(test) > (double)cT * 0.0001d)
            {
                CfreeRet = (decimal)((float)CfreeRet + Deltacf);
                if ((double)CfreeRet < 0.00000001d | cT == 0m)
                {
                    CfreeRet = 0m;
                    break;
                }
                ItcT = (decimal)((double)(w * CfreeRet / 1000m) + (double)gamma * Math.Pow((double)CfreeRet, 0.379d));
                test = cT - ItcT;
                if (test > 0m)
                {
                    if (SensPos == false)
                    {
                        Deltacf = -Deltacf / 10f;
                        SensPos = true;
                    }
                }
                else if (test < 0m)
                {
                    if (SensPos == true)
                    {
                        Deltacf = -Deltacf / 10f;
                        SensPos = false;
                    }
                }
            }

            return CfreeRet;
        }


        // 'Inversion de matrice pour le transport par convection des cl-
        // Public Sub InvMaBa(ByRef L() As Decimal, ByRef b() As Decimal, ByRef Ne As Short)
        // Dim i As Long
        // Dim j As Long
        // Dim k As Long
        // Dim n As Long = 2 * Ne - 2
        // Const m = 4
        // Dim b01 As Long
        // Dim b02 As Long
        // Dim a(n, 2 * m + 1) As Decimal

        // 'Construction de la matrice a
        // For i = 2 To n Step 2
        // If i <> 2 Then a(i, 1) = -1 / L(i / 2 - 1)
        // If i <> 2 Then a(i, 2) = 3 / L(i / 2 - 1)
        // a(i, 3) = 1 / L(i / 2)
        // a(i, 4) = 7 / L(i / 2)
        // a(i, 5) = 5 / L(i / 2 + 1)
        // a(i, 6) = a(i, 5)
        // If i <> n Then a(i, 7) = 3 / L(i / 2 + 2)
        // If i <> n Then a(i, 8) = 1 / L(i / 2 + 2)
        // Next
        // For i = 1 To n Step 2
        // If i <> 2 Then a(i, 2) = -L((i + 1) / 2) / L((i + 1) / 2 - 1)
        // If i <> 2 Then a(i, 3) = 3 * L((i + 1) / 2) / L((i + 1) / 2 - 1)
        // a(i, 4) = -3
        // a(i, 5) = -1
        // a(i, 6) = 3 * L((i + 1) / 2) / L((i + 1) / 2 + 1)
        // a(i, 7) = a(i, 6) / 3
        // Next
        // For i = 1 To 4
        // For j = 1 To 5 - i
        // a(i, j) = 0
        // a(n + 1 - i, 2 * m + 2 - j) = 0
        // Next
        // Next

        // For i = 2 To m + 1
        // a(1, m + i) = a(1, m + i) / a(1, m + 1)
        // Next
        // For k = 2 To n - 1
        // b01 = k - m
        // If b01 < 1 Then b01 = 1
        // For j = b01 To k - 1
        // a(k, m + 1) = a(k, m + 1) - a(k, m + j - k + 1) * a(j, m + k - j + 1)
        // Next
        // b01 = m + k
        // If b01 > n Then b01 = n
        // For i = k + 1 To b01
        // b02 = k - m
        // If b02 < 1 Then b02 = 1
        // For j = b02 To k - 1
        // If m + j - i + 1 >= 1 Then a(i, m + k - i + 1) = a(i, m + k - i + 1) - a(i, m + j - i + 1) * a(j, m + k - j + 1)
        // Next
        // For j = b02 To k - 1
        // If m + i - j + 1 <= 2 * m + 1 Then a(k, m + i - k + 1) = (a(k, m + i - k + 1) - a(k, m + j - k + 1) * a(j, m + i - j + 1))
        // Next
        // a(k, m + i - k + 1) = a(k, m + i - k + 1) / a(k, m + 1)
        // Next
        // Next
        // For j = n - m To n - 1
        // a(n, m + 1) = a(n, m + 1) - a(n, m + j - n + 1) * a(j, m + n - j + 1)
        // Next

        // b(1) = b(1) / a(1, m + 1)
        // For i = 2 To n
        // b02 = i - m
        // If b02 < 1 Then b02 = 1
        // For j = b02 To i - 1
        // b(i) = b(i) - a(i, m + j - i + 1) * b(j)
        // Next
        // b(i) = b(i) / a(i, m + 1)
        // Next
        // For i = n - 1 To 1 Step -1
        // b01 = m + i
        // If b01 > n Then b01 = n
        // For j = i + 1 To b01
        // b(i) = b(i) - a(i, m + j - i + 1) * b(j)
        // Next
        // Next
        // End Sub

    }
}