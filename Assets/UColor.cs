using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//The Useless Color Library
public class UColor
{
    /// <summary>
    /// Returns a complementary Color to the one passed in.
    /// </summary>
    /// <returns>
    /// The new complementary Color to the one passed in
    /// </returns>
    /// <param name='sourceColor'>
    /// The color you want to start with
    /// </param>
    //Returns the complimentary color of the source color
    public static Color Complement(Color sourceColor)
    {
        HSL hsl = HSL.fromColor(sourceColor);		//Convert to HSL
        hsl.Hue += 0.5f; 						//Change the hue value to the opposite
		
        //Do nothing to saturation and lightness
        if (hsl.Saturation < 0.01 || hsl.Lightness < 0.1) //Greyscale
        {
            hsl.Lightness = 1.0f - hsl.Lightness;
            hsl.Saturation = 1.0f - hsl.Saturation;
        }
		
		
		
        Color newColor = hsl.toColor(); 	//Convert this back to RGB
        newColor.a = sourceColor.a;			//Add back in the A value
		
        return newColor;
    } 

    public static Color RandomColor(RandomSeed r = null)
    {
        if (r == null)
        {
			
            r = new RandomSeed(DateTime.Now.Millisecond);
            Debug.Log("Ucolor.cs got a broken random seed request.");
        }

        UColor.HSL theColor = new UColor.HSL((float)r.getRandom(), 0.7f, 0.8f); //Choose a  color
        Color color = theColor.toColor();
        return color;
    }//RandomColor

    public static Color ColorRotate(Color sourceColor, float rotation)
    {
        HSL hsl = HSL.fromColor(sourceColor);		//Convert to HSL
        hsl.Hue += rotation; 						//Change the hue value to the opposite
		
        Color newColor = hsl.toColor(); 				//Convert this back to a Color
        newColor.a = sourceColor.a;					//Add back in the A value
		
        return newColor;
    }

    public static Color ChangeRelativeBrightness(Color sourceColor, float relativeBrightness)
    {
        HSL hsl = HSL.fromColor(sourceColor);		//Convert to HSL
		
        hsl.Lightness *= relativeBrightness;
		
        Color newColor = hsl.toColor(); 	//Convert this back to RGB
        newColor.a = sourceColor.a;			//Add back in the A value
		
        return newColor;
    }

    public static Color ChangeBrightness(Color sourceColor, float newBrightness)
    {
        HSL hsl = HSL.fromColor(sourceColor);		//Convert to HSL
		
        hsl.Lightness = newBrightness;
		
        Color newColor = hsl.toColor(); 	//Convert this back to RGB
        newColor.a = sourceColor.a;			//Add back in the A value
		
        return newColor;
    }

    public static Color ChangeRelativeSaturation(Color sourceColor, float relativeSaturation)
    {
        HSL hsl = HSL.fromColor(sourceColor);		//Convert to HSL
		
        hsl.Saturation *= relativeSaturation;
		
        Color newColor = hsl.toColor(); 	//Convert this back to RGB
        newColor.a = sourceColor.a;			//Add back in the A value
		
        return newColor;
    }
	
    public static Color ChangeSaturation(Color sourceColor, float newSaturation)
    {
        HSL hsl = HSL.fromColor(sourceColor);		//Convert to HSL
		
        hsl.Saturation = newSaturation;
		
        Color newColor = hsl.toColor(); 	//Convert this back to RGB
        newColor.a = sourceColor.a;			//Add back in the A value
		
        return newColor;
    }
	
    /// <summary>
    /// Returns a list of colors that are split complements of the source .
    /// </summary>
    public static List<Color> SplitCompliments(Color sourceColor)
    {
        List<Color> palette = new List<Color>();
		
        HSL hsl1 = HSL.fromColor(sourceColor);		//Convert to HSL
        HSL hsl2 = HSL.Copy(hsl1);
		
        hsl1.Hue += 0.4166f; 
        hsl2.Hue -= 0.4166f; 
		
        //Do nothing to saturation and lightness
		
        Color color1 = hsl1.toColor(); 	//Convert this back to RGB
        Color color2 = hsl2.toColor(); 	//Convert this back to RGB
		
        color1.a = sourceColor.a;			//Add back in the A value
        palette.Add(color1);
        color2.a = sourceColor.a;			//Add back in the A value
        palette.Add(color2);
		
        return palette;
    }
	
    /// <summary>
    /// Returns a list of colors that are triadic in relation to the source
    /// </summary>
    public static List<Color> Triadic(Color sourceColor)
    {
		
        List<Color> palette = new List<Color>();
		
        HSL hsl1 = HSL.fromColor(sourceColor);		//Convert to HSL
        HSL hsl2 = HSL.Copy(hsl1);
		
        hsl1.Hue += 0.33f; 
        hsl2.Hue -= 0.33f; 
		
        //Do nothing to saturation and lightness
		
        Color color1 = hsl1.toColor(); 	//Convert this back to RGB
        Color color2 = hsl2.toColor(); 	//Convert this back to RGB
		
        color1.a = sourceColor.a;			//Add back in the A value
        palette.Add(color1);
        color2.a = sourceColor.a;			//Add back in the A value
        palette.Add(color2);
		
        return palette;
    }

    /// <summary>
    /// Returns a list of colors that are monochromatic in relation to the source
    /// </summary>
    public static List<Color> Monochromatic(Color sourceColor, int howMany = 4)
    {
		
        List<Color> palette = new List<Color>();
		
        HSL source = HSL.fromColor(sourceColor);		//Convert to HSL
				
        for (int i=0; i < howMany; i++)
        {
            HSL hsl = HSL.Copy(source);			//Create a copy of the source HSL
            hsl.Lightness *= (howMany / 2 - i);	//Change the lightness of it
            Color color = hsl.toColor();		//Convert it back
            color.a = sourceColor.a;			//Add back in the Alpha value
            palette.Add(color);					//Add it to the palette
        }

        return palette;
    }
	
	
	
    /// <summary>
    /// Returns a list of colors that analagously related to the source .
    /// </summary>
    public static List<Color> Analagous(Color sourceColor, int howMany = 2)
    {
        List<Color> palette = new List<Color>();
		
        HSL source = HSL.fromColor(sourceColor);		//Convert to HSL
        float increment = 0.083f;
        float top = source.Hue + (increment * (howMany / 2));

        for (int i=0; i < howMany; i++)
        {
            HSL hsl = HSL.Copy(source);			//Create a copy of the source HSL
            hsl.Hue -= top - (i * increment);	//Change the lightness of it
            Color color = hsl.toColor();		//Convert it back
            color.a = sourceColor.a;			//Add back in the Alpha value
            palette.Add(color);					//Add it to the palette
        }
		
        return palette;


        /*
		List<Color> palette = new List<Color>();
		
		HSL hsl1 = HSL.fromColor(sourceColor);		//Convert to HSL
		HSL hsl2 = HSL.Copy(hsl1);
		
		hsl1.Hue += 0.083f; 
		hsl2.Hue -= 0.083f; 
		
		//Do nothing to saturation and lightness
		
		Color color1 = hsl1.toColor(); 	//Convert this back to RGB
		Color color2 = hsl2.toColor(); 	//Convert this back to RGB
		
		color1.a = sourceColor.a;			//Add back in the A value
		palette.Add(color1);
		color2.a = sourceColor.a;			//Add back in the A value
		palette.Add(color2);
		
		return palette;
		*/
    }
	
    /// <summary>
    /// A quick and easy HSL CLass
    /// </summary>
    public class HSL
    {
        private float h;
        private float s;
        private float l;
		
        public float Hue
        {
            get
            {
                return h;
            }
            set
            {
                h = value;
                h = wrap(h);
            }
        }
		
        public float Saturation
        {
            get
            {
                return s;
            }
            set
            {
                s = Mathf.Clamp01(value);
            }
        }
		
        public float Lightness
        {
            get
            {
                return l;
            }
            set
            {
                l = Mathf.Clamp01(value);
            }
        }
		
        public static float wrap(float source)
        {
            if (source < 1 && source > 0)
            {
                return source;
            }
            float wrappedNum = source;
            while (wrappedNum > 1)
            {
                wrappedNum -= 1.0f;
            }
			
            while (wrappedNum < 0)
            {
                wrappedNum += 1.0f;
            }
            return wrappedNum;
        }//wrap
		
        public HSL(float H, float S, float L)
        {
            h = H;
            s = S;
            l = L;
        }
		
        public string toString()
        {
            return toColor().ToString();
        }
		
        public static HSL Copy(HSL sourceHSL)
        {
            HSL copied = new HSL(sourceHSL.h, sourceHSL.s, sourceHSL.l);
            return copied;
        }
		
        public Color toColor()
        {
            float r = Lightness;
            float g = Lightness;
            float b = Lightness;
            if (Saturation != 0)
            {
                float max = Lightness;
                float dif = Lightness * Saturation;
                float min = Lightness - dif;
	 
                float h = Hue * 360f;
	 
                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                } else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                } else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                } else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                } else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                } else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                } else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }
 
            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), 1.0f);
        }
		
        private float Hue2RGB(float chroma1, float chroma2, float vHue)
        {
            if (vHue < 0)
                vHue += 1;
            if (vHue > 1)
                vHue -= 1;
            if ((6.0f * vHue) < 1)
                return (chroma1 + (chroma2 - chroma1) * 6.0f * vHue);
            if ((2.0f * vHue) < 1)
                return (chroma2);
            if ((3.0f * vHue) < 2)
                return (chroma1 + (chroma2 - chroma1) * ((2.0f / 3.0f) - vHue) * 6.0f);
            return (chroma1);
        }
		
        /// <summary>
        /// Creates an HSL object from a Unity Color object
        /// </summary>
        /// <returns>
        /// The HSL.
        /// </returns>
        /// <param name='sourceColor'>
        /// The Color object to turn into an HSL
        /// </param>
		
        public static HSL fromColor(Color sourceColor)
        {
            //Convert from RGBA to HSL
            float r = sourceColor.r;
            float g = sourceColor.g;
            float b = sourceColor.b;
            //float a = source.a;
			
            float hue = 0.0f;
            float sat = 0.0f;
            float lit = 0.0f;
			
            float Min = Mathf.Min(r, g, b);   //Min. value of RGB
            float Max = Mathf.Max(r, g, b);   //Max. value of RGB
            float delta = Max - Min;
			
            lit = Max;
			
            if (Max != 0)
                sat = ((float)(Max - Min)) / ((float)Max);
            else
                sat = 0;
			
            if (sat == 0)                     //This is a gray, no chroma...
            {
                hue = 0;			
            } else                                    //Chromatic data...
            {
                float del_R = ((Max - r)) / delta;
                float del_G = ((Max - g)) / delta;
                float del_B = ((Max - b)) / delta;
				
                if (r == Max) 
                    hue = del_B - del_G;
                else if (g == Max) 
                    hue = 2.0f + del_R - del_B;
                else if (b == Max) 
                    hue = 4.0f + del_G - del_R;
				
                hue = hue / 6;
					
                if (hue < 0) 
                    hue += 1;
				   
                if (hue > 1) 
                    hue -= 1;
            }
			
            return new HSL(hue, sat, lit);
        }//toHSL
    };
	
    //////////////////////////////////////////
    // HEX CONVERSION CRAP
    //////////////////////////////////////////
	
    public static string minStrByte(string byte1, string byte2)
    {
        int num1 = ByteStrToInt(byte1);
        int num2 = ByteStrToInt(byte2);
		
        int smallerNum = Math.Min(num1, num2);
		
        string a = GetHex(Mathf.FloorToInt(smallerNum / 16));
        string b = GetHex(Mathf.RoundToInt(smallerNum) % 16);
		
        return a + b;
		
    }
	
    private static int ByteStrToInt(string theByte)
    {
        return (HexToInt(theByte [0]) * 16) + HexToInt(theByte [1]);
    }
	
    private static string GetHex(int num)
    {
        const string alpha = "0123456789ABCDEF";
        string ret = "";
        ret = "" + alpha [num];
        
		
        return ret;
    }
 
    private static int HexToInt(char hexChar)
    {
        switch (hexChar)
        {
            case '0':
                return 0;
            case '1':
                return 1;
            case '2':
                return 2;
            case '3':
                return 3;
            case '4':
                return 4;
            case '5':
                return 5;
            case '6':
                return 6;
            case '7':
                return 7;
            case '8':
                return 8;
            case '9':
                return 9;
            case 'A':
                return 10;
            case 'B':
                return 11;
            case 'C':
                return 12;
            case 'D':
                return 13;
            case 'E':
                return 14;
            case 'F':
                return 15;
        }
        return -1;
    }
 
    public static string RGBToHex(Color color)
    {
        float red = color.r * 255;
        float green = color.g * 255;
        float blue = color.b * 255;
 
        string a = GetHex(Mathf.FloorToInt(red / 16));
        string b = GetHex(Mathf.RoundToInt(red) % 16);
        string c = GetHex(Mathf.FloorToInt(green / 16));
        string d = GetHex(Mathf.RoundToInt(green) % 16);
        string e = GetHex(Mathf.FloorToInt(blue / 16));
        string f = GetHex(Mathf.RoundToInt(blue) % 16);
 
        return a + b + c + d + e + f;
    }
	
	
 
    public static Color HexToRGB(string color)
    {
        float red = (HexToInt(color [1]) + HexToInt(color [0]) * 16f) / 255f;
        float green = (HexToInt(color [3]) + HexToInt(color [2]) * 16f) / 255f;
        float blue = (HexToInt(color [5]) + HexToInt(color [4]) * 16f) / 255f;
        Color finalColor = new Color { r = red, g = green, b = blue, a = 1 };
        return finalColor;
    }
		
}
