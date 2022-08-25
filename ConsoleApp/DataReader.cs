namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        private ImportedObject StringValuesToImportedObject(String[] values)
        {
            if(values == null)
                return null;

            var importedObject = new ImportedObject();
            for (int index = 0; index < values.Length; index++)
            {
                switch (index)
                {
                    case 0:
                        importedObject.Type = values[index];
                        break;
                    case 1:
                        importedObject.Name = values[index];
                        break;
                    case 2:
                        importedObject.Schema = values[index];
                        break;
                    case 3:
                        importedObject.ParentName = values[index];
                        break;
                    case 4:
                        importedObject.ParentType = values[index];
                        break;
                    case 5:
                        importedObject.DataType = values[index];
                        break;
                    case 6:
                        importedObject.IsNullable = values[index];
                        break;
                }
            }

            return importedObject;

        }

        private bool ImportData(string fileToImport)
        {
            ImportedObjects = new List<ImportedObject>();

            try
            {

                var streamReader = new StreamReader(fileToImport);

                var importedLines = new List<string>();

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    importedLines.Add(line);
                }

                for (int i = 0; i < importedLines.Count; i++)
                {
                    var importedLine = importedLines[i];
                    var values = importedLine.Split(';');

                    var importedObject = StringValuesToImportedObject(values);
                    if (importedObject != null)
                        ((List<ImportedObject>)ImportedObjects).Add(importedObject);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private void ClearData()
        {
            try
            {
                // clear and correct imported data
                foreach (var importedObject in ImportedObjects)
                {
                    if (importedObject.Type != null)
                        importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                    if (importedObject.Name != null)
                        importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    if (importedObject.Schema != null)
                        importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    if (importedObject.ParentName != null)
                        importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    if (importedObject.ParentType != null)
                        importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AssignNumberOfChildren()
        {
            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type)
                    {
                        if (impObj.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                        }
                    }
                }
            }
        }

        private void PrintColumn(ImportedObject column, ImportedObject table)
        {
            if (column.ParentType == null)
                return;

            if (column.ParentType.ToUpper() == table.Type)
            {
                if (column.ParentName == table.Name)
                {
                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                }
            }
        }

        private void PrintDatabase(ImportedObject table, ImportedObject database)
        {
            if (table.ParentType == null)
                return;

            if (table.ParentType.ToUpper() == database.Type)
            {
                if (table.ParentName == database.Name)
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    // print all table's columns
                    foreach (var column in ImportedObjects)
                    {
                        PrintColumn(column, table);
                    }
                }
            }
        }

        private void PrintData()
        {
            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        PrintDatabase(table, database);
                    }
                }
            }
        }

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            if(ImportData(fileToImport))
            {
                //order matters
                ClearData();
                AssignNumberOfChildren();
                PrintData();
            }
            Console.ReadLine();
        }
    }

}
