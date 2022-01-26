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
using System.Data;

namespace Sys.Data
{
	public interface IDbAccess
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		int ExecuteNonQuery();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		object ExecuteScalar();

		/// <summary>
		/// 
		/// </summary>
		void ExecuteTransaction();

		/// <summary>
		/// Retrieve data set
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns> The number of rows successfully added to or refreshed in the System.Data.DataSet.</returns>
		int FillDataSet(DataSet dataSet);

		/// <summary>
		/// Retrieve data table
		/// </summary>
		/// <param name="dataTable">The System.Data.DataTable objects to fill from the data source.</param>
		/// <param name="startRecord">The zero-based record number to start with.</param>
		/// <param name="maxRecords">The maximum number of records to retrieve.</param>
		/// <returns></returns>
		int FillDataTable(DataTable dataTable, int startRecord, int maxRecords);

		/// <summary>
		/// Read bulk data set
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		int ReadDataSet(DataSet dataSet);

		/// <summary>
		/// Read bulk data table 
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="startRecord"></param>
		/// <param name="maxRecords"></param>
		/// <returns></returns>
		int ReadDataTable(DataTable dataTable, int startRecord, int maxRecords);
	}
}