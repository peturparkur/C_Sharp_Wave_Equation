using System;

public class FluidSimFlat
{
    public Matrix<float> val; //the matrix represents the flat 2D surface and each value is a height/density value associated to each position at T
    Matrix<float> prevVal; //values at T-1
    Matrix<float> pastVal; //values at T-2
    public float dx; //the defined difference of each index of vals
    public float dy;
    //public float dt; //the defined time difference between val and prevVal
    //public float c;

    //wave equation is defined as ddot(u) = c^2 * delta^2(u); where delta is the Del or Delta operator (gradient)

    public FluidSimFlat(Matrix<float> _val, Matrix<float> _prevVal, float _dx, float _dy)
    {
        val = new Matrix<float>(_val);
        prevVal = new Matrix<float>(_val);
        //c = _c;
        dx = _dx;
        dy = _dy;
    }

    public FluidSimFlat(int sizeX, int sizeY, float _dx, float _dy)
    {
        val = new Matrix<float>(sizeX, sizeY);
        prevVal = new Matrix<float>(sizeX, sizeY);
        pastVal = new Matrix<float>(sizeX, sizeY);
        //c = _c;
        dx = _dx;
        dy = _dy;
    }

    /*public void Initialise(int sizeX, int sizeY, float _c)
    {
        val = new Matrix<float>(sizeX, sizeY);
        prevVal = new Matrix<float>(sizeX, sizeY);
        pastVal = new Matrix<float>(sizeX, sizeY);
        c = _c;
    }*/

    /*public void Initialise(Matrix<float> _val, Matrix<float> _prevVal, float _c)
    {
        val = new Matrix<float>(_val);
        prevVal = new Matrix<float>(_val);
        c = _c;
    }*/

    public float Accelaration(int x, int y, float c, float dt, float d) //this calculates ddot(u), the accelaration of u
    {
        //X,Y represent the row and column position respectively
        //if val.rows = n then we only allow x to be n-2 instead of the n-1 which would be the last value
        int n = val.rows;
        int m = val.columns;

        if (y >= m - 1 || y <= 0) throw new Exception("Y is on/outside the boundary");
        if (x >= n - 1 || x <= 0) throw new Exception("X is on/outside the boundary");

        //since we work in 2D delta^2 ==> d(u)/dx^2 + d(u)/dy^2

        float nx = val[x + 1, y] - 2f * val[x, y] + val[x - 1, y]; //x derivative
        float ny = val[x, y + 1] - 2f * val[x, y] + val[x, y - 1]; //y derivative

        float dr2 = dx * dx + dy * dy;
        float dr = (float)Math.Sqrt(dr2);
        float fx = (val[x - 1, y - 1] - 2f * val[x, y] + val[x + 1, y + 1]);
        float fy = (val[x + 1, y - 1] - 2f * val[x, y] + val[x - 1, y + 1]);
        //float mult = (dx * dx) / dr2;

        float average = (val[x + 1, y] + val[x - 1, y])/dx + (val[x, y + 1] + val[x, y - 1])/dy + (val[x+1,y+1] + val[x+1,y-1] + val[x-1,y+1] + val[x-1,y-1])/dr;
        average = average / 8f;

        return c * c * (nx/(dx*dx) + ny/(dy*dy) + (fx+fy)/dr2) - d*(val[x,y] - prevVal[x,y])/dt; //this is c^2 * delta^2(u);
    }

    public Matrix<float> AccelerationMatrix(float c, float dt, float d)
    {
        Matrix<float> acc = new Matrix<float>(val.rows, val.columns);
        for(int i=1; i<val.rows-1; i++)
        {
            for(int j=1; j<val.columns-1; j++)
            {
                acc[i, j] = Accelaration(i, j, c,dt,d);
            }
        }
        return acc;
    }

    public void Step(float dt, float c, float d)
    {
        Matrix<float> acc = AccelerationMatrix(c,dt,d);
        Matrix<float> temp = new Matrix<float>(val);
        for(int i=1; i<val.rows-1; i++)
        {
            for(int j=1; j<val.columns-1; j++)
            {
                val[i, j] = 2f * val[i, j] - prevVal[i, j] + c * c * acc[i, j] * dt * dt; //verlet method
            }
        }
        prevVal = temp; //to assign the pervious values
    }

}
