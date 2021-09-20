//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//

namespace Sys.Data.Text
{
    /// <summary>
    /// Date interval defined in SQL Server
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// yyyy, yy = Year
        /// </summary>
        year,

        /// <summary>
        /// qq, q = Quarter
        /// </summary>
        quarter,

        /// <summary>
        /// mm, m = month
        /// </summary>
        month,

        /// <summary>
        /// dy, y = Day of the year
        /// </summary>
        dayofyear,

        /// <summary>
        /// dd, d = Day
        /// </summary>
        day,

        /// <summary>
        /// ww, wk = Week
        /// </summary>
        week,

        /// <summary>
        /// dw, w = Weekday
        /// </summary>
        weekday,

        /// <summary>
        /// hh = hour
        /// </summary>
        hour,

        /// <summary>
        /// mi, n = Minute
        /// </summary>
        minute,

        /// <summary>
        /// ss, s = Second
        /// </summary>
        second,

        /// <summary>
        /// ms = Millisecond
        /// </summary>
        millisecond, 
    }
}
