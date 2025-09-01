using Autodesk.Revit.DB;
using System;

namespace GeoJsonImporter.Work.Utils
{
    /// <summary>
    /// Zentrale Einheitenkonvertierung für Revit Plugin
    /// Revit arbeitet intern mit Feet - alle externen Daten sind in Meter
    /// </summary>
    public static class UnitConverter
    {
        #region Constants
        
        /// <summary>
        /// Revit interne Einheiten: 1 Fuß = 0.3048 Meter (exakt)
        /// </summary>
        public const double FeetToMeters = 0.3048;
        
        /// <summary>
        /// Umkehrung: 1 Meter = 3.28084 Fuß (exakt)
        /// </summary>
        public const double MetersToFeet = 1.0 / FeetToMeters;
        
        /// <summary>
        /// Quadratfuß zu Quadratmeter
        /// </summary>
        public const double SquareFeetToSquareMeters = FeetToMeters * FeetToMeters;
        
        /// <summary>
        /// Quadratmeter zu Quadratfuß
        /// </summary>
        public const double SquareMetersToSquareFeet = MetersToFeet * MetersToFeet;
        
        /// <summary>
        /// Kubikfuß zu Kubikmeter
        /// </summary>
        public const double CubicFeetToCubicMeters = FeetToMeters * FeetToMeters * FeetToMeters;
        
        /// <summary>
        /// Kubikmeter zu Kubikfuß
        /// </summary>
        public const double CubicMetersToCubicFeet = MetersToFeet * MetersToFeet * MetersToFeet;
        
        #endregion
        
        #region Linear Conversions
        
        /// <summary>
        /// Konvertiert Revit-Skalareinheit (Imperial, in Fuß) in Meter
        /// </summary>
        /// <param name="feet">Wert in Fuß (Revit intern)</param>
        /// <returns>Wert in Meter</returns>
        public static double FeetToMetric(double feet) => feet * FeetToMeters;
        
        /// <summary>
        /// Konvertiert Meter in Revit-Skalareinheit (Imperial, in Fuß)
        /// </summary>
        /// <param name="meters">Wert in Meter</param>
        /// <returns>Wert in Fuß (Revit intern)</returns>
        public static double MetricToFeet(double meters) => meters * MetersToFeet;
        
        /// <summary>
        /// Konvertiert Millimeter in Revit-Fuß
        /// </summary>
        /// <param name="millimeters">Wert in Millimeter</param>
        /// <returns>Wert in Fuß (Revit intern)</returns>
        public static double MillimetersToFeet(double millimeters) => (millimeters / 1000.0) * MetersToFeet;
        
        /// <summary>
        /// Konvertiert Revit-Fuß in Millimeter
        /// </summary>
        /// <param name="feet">Wert in Fuß (Revit intern)</param>
        /// <returns>Wert in Millimeter</returns>
        public static double FeetToMillimeters(double feet) => feet * FeetToMeters * 1000.0;
        
        #endregion
        
        #region Area Conversions
        
        /// <summary>
        /// Konvertiert Quadratfuß in Quadratmeter
        /// </summary>
        public static double SquareFeetToMetric(double squareFeet) => squareFeet * SquareFeetToSquareMeters;
        
        /// <summary>
        /// Konvertiert Quadratmeter in Quadratfuß
        /// </summary>
        public static double SquareMetricToFeet(double squareMeters) => squareMeters * SquareMetersToSquareFeet;
        
        #endregion
        
        #region Volume Conversions
        
        /// <summary>
        /// Konvertiert Kubikfuß in Kubikmeter
        /// </summary>
        public static double CubicFeetToMetric(double cubicFeet) => cubicFeet * CubicFeetToCubicMeters;
        
        /// <summary>
        /// Konvertiert Kubikmeter in Kubikfuß
        /// </summary>
        public static double CubicMetricToFeet(double cubicMeters) => cubicMeters * CubicMetersToCubicFeet;
        
        #endregion
        
        #region XYZ Point Conversions
        
        /// <summary>
        /// Konvertiert Meter-Koordinaten in Revit XYZ (Fuß)
        /// </summary>
        /// <param name="x_m">X-Koordinate in Meter</param>
        /// <param name="y_m">Y-Koordinate in Meter</param>
        /// <param name="z_m">Z-Koordinate in Meter</param>
        /// <returns>Revit XYZ in Fuß</returns>
        public static XYZ ToRevitXYZ(double x_m, double y_m, double z_m)
        {
            return new XYZ(
                MetricToFeet(x_m),
                MetricToFeet(y_m),
                MetricToFeet(z_m)
            );
        }
        
        /// <summary>
        /// Konvertiert Revit XYZ (Fuß) in Meter-Koordinaten
        /// </summary>
        /// <param name="point">Revit XYZ Point in Fuß</param>
        /// <param name="x_m">X-Koordinate in Meter (out)</param>
        /// <param name="y_m">Y-Koordinate in Meter (out)</param>
        /// <param name="z_m">Z-Koordinate in Meter (out)</param>
        public static void FromRevitXYZ(XYZ point, out double x_m, out double y_m, out double z_m)
        {
            x_m = FeetToMetric(point.X);
            y_m = FeetToMetric(point.Y);
            z_m = FeetToMetric(point.Z);
        }
        
        /// <summary>
        /// Konvertiert Revit XYZ (Fuß) in Meter-Koordinaten
        /// </summary>
        /// <param name="point">Revit XYZ Point in Fuß</param>
        /// <returns>Tupel mit (x_m, y_m, z_m) in Meter</returns>
        public static (double x_m, double y_m, double z_m) FromRevitXYZ(XYZ point)
        {
            return (
                FeetToMetric(point.X),
                FeetToMetric(point.Y),
                FeetToMetric(point.Z)
            );
        }
        
        /// <summary>
        /// Konvertiert UTM-Koordinaten (Meter) in Revit XYZ (Fuß)
        /// </summary>
        /// <param name="utmX">UTM X (Easting) in Meter</param>
        /// <param name="utmY">UTM Y (Northing) in Meter</param>
        /// <param name="elevation">Höhe in Meter</param>
        /// <returns>Revit XYZ in Fuß</returns>
        public static XYZ UtmToRevitXYZ(double utmX, double utmY, double elevation)
        {
            return ToRevitXYZ(utmX, utmY, elevation);
        }
        
        /// <summary>
        /// Konvertiert Revit XYZ (Fuß) in UTM-Koordinaten (Meter)
        /// </summary>
        /// <param name="point">Revit XYZ Point in Fuß</param>
        /// <returns>Tupel mit (utmX, utmY, elevation) in Meter</returns>
        public static (double utmX, double utmY, double elevation) RevitXYZToUtm(XYZ point)
        {
            return FromRevitXYZ(point);
        }
        
        #endregion
        
        #region Precision Helpers
        
        /// <summary>
        /// Rundet Meter-Werte auf UTM-Grid-Präzision (0.001m = 1mm)
        /// </summary>
        /// <param name="meters">Wert in Meter</param>
        /// <returns>Gerundeter Wert auf Millimeter-Präzision</returns>
        public static double RoundToMillimeter(double meters)
        {
            return Math.Round(meters, 3);
        }
        
        /// <summary>
        /// Rundet Meter-Werte auf Zentimeter-Präzision
        /// </summary>
        /// <param name="meters">Wert in Meter</param>
        /// <returns>Gerundeter Wert auf Zentimeter-Präzision</returns>
        public static double RoundToCentimeter(double meters)
        {
            return Math.Round(meters, 2);
        }
        
        /// <summary>
        /// Rundet UTM-Koordinaten auf 100-Meter-Grid
        /// </summary>
        /// <param name="utmCoordinate">UTM-Koordinate in Meter</param>
        /// <returns>Auf 100m gerundete Koordinate</returns>
        public static double RoundToUtmGrid100m(double utmCoordinate)
        {
            return Math.Round(utmCoordinate / 100.0) * 100.0;
        }
        
        #endregion
        
        #region Validation Helpers
        
        /// <summary>
        /// Prüft ob ein Wert in einem vernünftigen UTM-Bereich liegt
        /// </summary>
        /// <param name="utmX">UTM X (Easting)</param>
        /// <param name="utmY">UTM Y (Northing)</param>
        /// <returns>True wenn Koordinaten plausibel sind</returns>
        public static bool IsValidUtmCoordinate(double utmX, double utmY)
        {
            // UTM Zone 33N (Deutschland) - grobe Plausibilitätsprüfung
            return utmX >= 200000 && utmX <= 800000 &&
                   utmY >= 5000000 && utmY <= 6000000;
        }
        
        /// <summary>
        /// Prüft ob eine Höhe plausibel ist (für Deutschland)
        /// </summary>
        /// <param name="elevation">Höhe in Meter</param>
        /// <returns>True wenn Höhe plausibel ist</returns>
        public static bool IsValidElevation(double elevation)
        {
            // Deutschland: -5m (Nordsee) bis 3000m (Alpen)
            return elevation >= -10.0 && elevation <= 3500.0;
        }
        
        #endregion
        
        #region Display Helpers
        
        /// <summary>
        /// Formatiert Meter-Werte für Anzeige
        /// </summary>
        /// <param name="meters">Wert in Meter</param>
        /// <param name="decimals">Anzahl Dezimalstellen</param>
        /// <returns>Formatierter String mit "m" Einheit</returns>
        public static string FormatMeters(double meters, int decimals = 3)
        {
            return $"{Math.Round(meters, decimals).ToString($"F{decimals}")} m";
        }
        
        /// <summary>
        /// Formatiert UTM-Koordinaten für Anzeige
        /// </summary>
        /// <param name="utmX">UTM X (Easting)</param>
        /// <param name="utmY">UTM Y (Northing)</param>
        /// <param name="elevation">Höhe in Meter</param>
        /// <returns>Formatierter UTM-String</returns>
        public static string FormatUtmCoordinates(double utmX, double utmY, double elevation)
        {
            return $"UTM: {utmX:F0}, {utmY:F0}, {elevation:F3}m";
        }
        
        #endregion
    }
}
