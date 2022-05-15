﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AppResources
{
    public static class ResourceManager
    {
        public static KeyValuePair<Stream, string> GetImageWithHexColor(string imageId, int feelTemp, string description = null)
        {
            string imagePath, color;

            switch (imageId)
            {
                case "01n":
                case "02n":
                    if (feelTemp > 0)
                    {
                        color = "#FF001338";
                        imagePath = "ClearSkySummerNight.jpg";
                    }
                    else
                    {
                        color = "#FF001E20";
                        imagePath = "ClearSkyWinterNight.jpg";
                    }
                    break;
                case "03n":
                case "04n":
                    color = "#FF000620";
                    imagePath = "CloudsNight.jpg";
                    break;
                case "03d":
                case "04d":
                    if (feelTemp < 16)
                    {
                        color = "#FF234994";
                        imagePath = "CloudyWinterDay.jpg";
                    }
                    else
                    {
                        color = "#FFAB6D49";
                        imagePath = "CloudySummerDay.jpg";
                    }
                    break;
                case "01d":
                case "02d":
                    if (feelTemp < 8)
                    {
                        color = "#FF1952C2";
                        imagePath = "ClearSkyWinterDay.jpg";
                    }
                    else if (feelTemp < 16)
                    {
                        color = "#FF1D83C0";
                        imagePath = "ClearSkySpringDay.jpg";
                    }
                    else
                    {
                        color = "#FFEB4A4A";
                        imagePath = "ClearSkySummerDay.jpeg";
                    }
                    break;
                case "09d":
                    color = "#FF7C8185";
                    imagePath = "ShowerRainDay.jpg";
                    break;
                case "09n":
                    color = "#FF040411";
                    imagePath = "ShowerRainNight.jpg";
                    break;
                case "10n":
                    color = "#FF010D31";
                    imagePath = "RainNight.jpg";
                    break;
                case "10d":
                    color = "#FF888888";
                    imagePath = "RainDay.jpg";
                    break;
                case "11d":
                    color = "#FF1D2329";
                    imagePath = "ThunderstormDay.jpg";
                    break;
                case "11n":
                    color = "#FF602B55";
                    imagePath = "ThunderstormNight.jpg";
                    break;
                case "13d":
                    color = "#FF99B5CB";
                    imagePath = "SnowDay.jpg";
                    break;
                case "13n":
                    color = "#FF27636F";
                    imagePath = "SnowNight.jpg";
                    break;
                case "50d":
                    if (!string.IsNullOrEmpty(description) && (description.ToLower().Contains("sand") ||
                        description.ToLower().Contains("piasek") || description.ToLower().Contains("dust") ||
                        description.ToLower().Contains("pył")))
                    {
                        color = "#FFD0B097";
                        imagePath = "SandDay.jpg";
                    }
                    else if (!string.IsNullOrEmpty(description) && (description.ToLower().Contains("volcanic") ||
                             description.ToLower().Contains("wulkaniczny")))
                    {
                        color = "#FF475965";
                        imagePath = "VolcanicAsh.jpg";
                    }
                    else if (!string.IsNullOrEmpty(description) && description.ToLower().Contains("tornado"))
                    {
                        color = "#FF385A5C";
                        imagePath = "Tornado.jpg";
                    }
                    else
                    {
                        if (feelTemp > 8)
                        {
                            color = "#FFADADAD";
                            imagePath = "FoggySummerDay.jpg";
                        }
                        else
                        {
                            color = "#FF428FFF";
                            imagePath = "FoggyWinterDay.jpg";
                        }
                    }
                    break;
                case "50n":
                    if (!string.IsNullOrEmpty(description) && (description.ToLower().Contains("sand") ||
                        description.ToLower().Contains("piasek") || description.ToLower().Contains("dust") ||
                        description.ToLower().Contains("pył")))
                    {
                        color = "#FFC96B25";
                        imagePath = "SandNight.jpg";
                    }
                    else if (!string.IsNullOrEmpty(description) && (description.ToLower().Contains("volcanic") ||
                            description.ToLower().Contains("wulkaniczny")))
                    {
                        color = "#FF475965";
                        imagePath = "VolcanicAsh.jpg";
                    }
                    else if (!string.IsNullOrEmpty(description) && description.ToLower().Contains("tornado"))
                    {
                        color = "#FF385A5C";
                        imagePath = "Tornado.jpg";
                    }
                    else
                    {
                        color = "#FF1F243C";
                        imagePath = "FoggyNight.jpg";
                    }
                    break;
                default:
                    throw new ArgumentException($"Invalid input data. There is no appropriate content for Id: {imageId}");
            }

            return new KeyValuePair<Stream, string>(Assembly.GetExecutingAssembly().GetManifestResourceStream($"AppResources.Images.{imagePath}"), color);
        }

        public static Stream GetIcon(string iconId)
        {
            string iconPath;

            switch (iconId)
            {
                case "01d":
                    iconPath = "ClearSkyDay.ico";
                    break;
                case "02d":
                    iconPath = "FewCloudsDay.ico";
                    break;
                case "03d":
                    iconPath = "ScatteredCloudsDay.ico";
                    break;
                case "04d":
                    iconPath = "BrokenCloudsDay.ico";
                    break;
                case "01n":
                    iconPath = "ClearSkyNight.ico";
                    break;
                case "02n":
                    iconPath = "FewCloudsNight.ico";
                    break;
                case "03n":
                    iconPath = "ScatteredCloudsNight.ico";
                    break;
                case "04n":
                    iconPath = "BrokenCloudsNight.ico";
                    break;
                case "09n":
                case "09d":
                    iconPath = "ShowerRain.ico";
                    break;
                case "10n":
                    iconPath = "RainNight.ico";
                    break;
                case "10d":
                    iconPath = "RainDay.ico";
                    break;
                case "11d":
                case "11n":
                    iconPath = "Thunderstorm.ico";
                    break;
                case "13d":
                case "13n":
                    iconPath = "Snow.ico";
                    break;
                case "50d":
                case "50n":
                    iconPath = "Mist.ico";
                    break;
                case "Update":
                    iconPath = "Update.ico";
                    break;
                default:
                    iconPath = "NoConnection.ico";
                    break;
            }

            return Assembly.GetExecutingAssembly().GetManifestResourceStream($"AppResources.Icons.{iconPath}");
        }

        public static Stream GetLanguage(string languageName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string languagePath;

            switch (languageName.ToLower())
            {
                case "polish":
                case "pl":
                    languagePath = "Polish.xml";
                    break;
                case "english":
                case "en":
                    languagePath = "English.xml";
                    break;
                default:
                    throw new ArgumentException($"Invalid input data. There is no appropriate content for language: {languageName}");
            }

            return assembly.GetManifestResourceStream($"AppResources.Languages.{languagePath}");
        }

        public static Stream GetUnits(string unitsName)
        {
            string unitsPath;

            switch (unitsName.ToLower())
            {
                case "standard":
                    unitsPath = "Standard.xml";
                    break;
                case "metric":
                    unitsPath = "Metric.xml";
                    break;
                case "imperial":
                    unitsPath = "Imperial.xml";
                    break;
                default:
                    throw new ArgumentException($"Invalid input data. There is no appropriate content for units: {unitsName}");
            }

            return Assembly.GetExecutingAssembly().GetManifestResourceStream($"AppResources.Units.{unitsPath}");
        }
    }
}
