using UnityEngine;
using System;
using System.Collections;

public class RandomSeed 
{
	/**
	 *	You can set the seed to any number in order to start your random number sequence
	 */
	private double seed;
    private double basicSeed;

	public RandomSeed(double seed) 
	{
        setSeed(seed);
	}//RandomSeed

	public double getSeed()
	{
        return basicSeed;
	}//getSeed
	
	public void setSeed(double newSeed)
	{
		seed = (double)Mathf.Abs((int)newSeed);
        basicSeed = newSeed;
	}//setSeed
	/**
	 *	Returns a pseudo-random number n, where 0 <= n < 1
	 */
	public double getRandom() 
	{
		seed = (seed*9301+49297) % 233280;
		return seed/(233280.0);
	}//getRandom


	/**
	 *	Utility method for getting real numbers in the provided range
	 *	The range is inclusive	 
	 */
	public float getFloatInRange(float bottom,float top) 
	{
		float dif = top-bottom+1;
		double num = getRandom();
		return (float)(bottom+(dif*num));
	}//getNumInRange

	/**
	 *	Utility method for getting integers numbers in the provided range
	 *	The range is inclusive
	 */
	public int getIntInRange(int bottom,int top) 
    {
		float dif = top-bottom+1;
		double num = getRandom();
		return (int)Mathf.Floor((float)(bottom+(dif*num)));
	}//getIntInRange

	public uint getRGBA()
	{
		return (uint)(0xFF000000 + getIntInRange(0,0xFFFFFF));
	}//getRGBA

	public bool getBoolean() 
	{
		return getRandom() < .5;
	}//getBoolean
	
	public bool percentageChance(float percent)
	{
		if(getFloatInRange(0,99) <= percent)
			return true;

		return false;
	}//percentageChance

    public string getChar(bool isUppercase = false)
    {
        int num = getIntInRange(0, 26); // Zero to 25
        string let = ((char)('a' + num)) + "";

        if(isUppercase)
            let = let.ToUpper();

        return let;
    }//getChar
	
}//RandomSeed
