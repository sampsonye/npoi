/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   2012 - Alfresco Software, Ltd.
   Alfresco Software has modified source of this file
   The details of Changes as svn diff can be found in svn at location root/projects/3rd-party/src 
==================================================================== */
namespace NPOI.SS.UserModel
{
    using NPOI.SS.Util;
    using NPOI.Util;
    using System;
    using System.Globalization;
    using System.Text;

    /**
     * A format that formats a double as Excel would, ignoring FieldPosition.
     * All other operations are unsupported.
     **/
    public class ExcelGeneralNumberFormat : FormatBase
    {

        private static long serialVersionUID = 1L;

        //private static MathContext TO_10_SF = new MathContext(10, RoundingMode.HALF_UP);

        //private DecimalFormatSymbols decimalSymbols;
        private NumberFormatInfo decimalSymbols;
        private DecimalFormat integerFormat;
        private DecimalFormat decimalFormat;
        private DecimalFormat scientificFormat;
        private CultureInfo culture;
        public ExcelGeneralNumberFormat(CultureInfo culture)
        {
            decimalSymbols = culture.NumberFormat;// DecimalFormatSymbols.GetInstance(locale);
            scientificFormat = new DecimalFormat("0.#####E0");//, decimalSymbols);
            //DataFormatter.ExcelStyleRoundingMode = (/*setter*/scientificFormat);
            integerFormat = new DecimalFormat("#");//, decimalSymbols);
            //DataFormatter.SetExcelStyleRoundingMode(/*setter*/integerFormat);
            decimalFormat = new DecimalFormat("#.##########");//, decimalSymbols);
            //DataFormatter.ExcelStyleRoundingMode = (/*setter*/decimalFormat);
            this.culture = culture;
        }

        public override StringBuilder Format(Object number, StringBuilder toAppendTo, CultureInfo culture)
        {
            double value;
            if (Number.IsNumber(number))
            {
                value = ((double)number);
                if (Double.IsInfinity(value) || Double.IsNaN(value))
                {
                    return integerFormat.Format(number, toAppendTo, culture);
                }
            }
            else
            {
                // testBug54786 Gets here with a date, so retain previous behaviour
                return integerFormat.Format(number, toAppendTo, culture);
            }

            double abs = Math.Abs(value);
            if (abs >= 1E11 || (abs <= 1E-10 && abs > 0))
            {
                return scientificFormat.Format(number, toAppendTo, culture);
            }
            else if (Math.Floor(value) == value || abs >= 1E10)
            {
                // integer, or integer portion uses all 11 allowed digits
                return integerFormat.Format(number, toAppendTo, culture);
            }
            // Non-integers of non-scientific magnitude are formatted as "up to 11
            // numeric characters, with the decimal point counting as a numeric
            // character". We know there is a decimal point, so limit to 10 digits.
            // https://support.microsoft.com/en-us/kb/65903
            //double rounded = new BigDecimal(value).round(TO_10_SF);
            double rounded = Math.Round(value, MidpointRounding.ToEven);
            return decimalFormat.Format(rounded, toAppendTo, culture);
        }

        public override string Format(object obj)
        {
            return double.Parse(obj.ToString()).ToString(culture.NumberFormat);
        }

        public override Object ParseObject(String source, int pos)
        {
            throw new InvalidOperationException();
        }

    }

}