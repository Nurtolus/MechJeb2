using System;

namespace MuMech {
    public delegate double BrentFun(double x, object o);

    // Brent's rootfinding method
    //
    // Uses secant or inverse quadratic interpolation as appropriate, with a fallback to bisection if necessary.
    //
    // f - 1-dimensional BrentFun function to find the root on
    // a - first guess of x value
    // b - second guess of x value
    // rtol - tolerance to solve to (1e-4 or something like that)
    // x - output of solved value
    // y - output of the function at the solved value (should be zero to rtol)
    // o - object to be passed to BrentFun for extra data (may be null)
    // maxiter - cap on iterations (will throw TimeoutException if exceeded)
    //
    public class Brent {
        public static void Solve(BrentFun f, double a, double b, double rtol, out double x, out double y, object o, int maxiter = 50)
        {
            double c = 0;
            double d = Double.MaxValue;

            double fa = f(a, o);
            double fb = f(b, o);

            double fc = 0;
            double s = 0;
            double fs = 0;

            if (fa * fb >= 0)
                throw new ArgumentException("Brent's rootfinding method: guess does not bracket the root");

            if (Math.Abs(fa) < Math.Abs(fb))
            {
                double tmp = a;
                a = b;
                b = tmp;
                tmp = fa;
                fa = fb;
                fb = tmp;
            }

            c = a;
            fc = fa;
            bool mflag = true;
            int i = 0;

            while (!(fb==0) && (Math.Abs(a-b) > rtol))
            {
                if ((fa != fc) && (fb != fc))
                    // inverse quadratic interpolation
                    s = a * fb * fc / (fa - fb) / (fa - fc) + b * fa * fc / (fb - fa) /
                        (fb - fc) + c * fa * fb / (fc - fa) / (fc - fb);
                else
                    // secant
                    s = b - fb * (b - a) / (fb - fa);

                double tmp2 = (3 * a + b) / 4;
                if ((!(((s > tmp2) && (s < b)) || ((s < tmp2) && (s > b)))) ||
                        (mflag && (Math.Abs(s - b) >= (Math.Abs(b - c) / 2))) ||
                        (!mflag && (Math.Abs(s - b) >= (Math.Abs(c - d) / 2))))
                {
                    // bisection
                    s = (a + b) / 2;
                    mflag = true;
                }
                else
                {
                    if ((mflag && (Math.Abs(b - c) < rtol)) || (!mflag && (Math.Abs(c - d) < rtol)))
                    {
                        s = (a + b) / 2;
                        mflag = true;
                    }
                    else
                    {
                        mflag = false;
                    }
                }

                fs = f(s, o);
                d = c;
                c = b;
                fc = fb;

                if (fa * fs < 0)
                {
                    b = s;
                    fb = fs;
                }
                else
                {
                    a = s;
                    fa = fs;
                }

                if (Math.Abs(fa) < Math.Abs(fb))
                {
                    double tmp = a;
                    a = b;
                    b = tmp;
                    tmp = fa;
                    fa = fb;
                    fb = tmp;
                }

                if (i++ >= maxiter)
                    throw new TimeoutException("Brent's rootfinding method: maximum iterations exceeded");
            }

            x = b;
            y = fb;
        }
    }
}
