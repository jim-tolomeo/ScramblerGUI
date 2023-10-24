using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace ScramblerGUI
{
    public class Scrambler
    {
        #region Column Headers

        private static string COL_DosageID = "Dosage ID";
        private static string COL_DosageDesc = "Dosage Description";

        private static readonly List<FileColumn> INPUT_FILE_COLUMNS = new List<FileColumn>() {
            new FileColumn() { Name = COL_DosageID, Required = true, Index = -1 },
            new FileColumn() { Name = COL_DosageDesc, Required = true, Index = -1 },
        };

        #endregion

        public void Scramble(string inputFile, Label lblProgress)
        {
            lblProgress.Text = "Parsing input file...";
            Application.DoEvents();

            var fileData = new List<string[]>();
            string[]? header = null;

            using (var parser = new TextFieldParser(inputFile, Encoding.UTF8))
            {
                parser.SetDelimiters(new[] { "\t" });

                var lineNbr = 0;

                while (!parser.EndOfData)
                {
                    lineNbr++;

                    lblProgress.Text = $"Parsing input file (line #{lineNbr})...";
                    Application.DoEvents();

                    var rowData = parser.ReadFields();

                    if (lineNbr == 1)
                    {
                        ValidateColumnHeaders(INPUT_FILE_COLUMNS, rowData);
                        header = rowData;
                    }
                    else
                        fileData.Add(rowData);
                }
            }

            //extract data to scrable...
            lblProgress.Text = "Extracting data to scramble...";
            Application.DoEvents();

            var scrambleData = new List<string[]>();

            fileData.ForEach(data =>
            {
                var scrambleDataList = new List<string>();

                INPUT_FILE_COLUMNS.ForEach(fileColumn => {
                    scrambleDataList.Add(data[fileColumn.Index]);
                    data[fileColumn.Index] = "SCRAMBLING!";
                });

                scrambleData.Add(scrambleDataList.ToArray());

            });


            //shuffle...
            lblProgress.Text = "Shuffling data...";
            Application.DoEvents();

            scrambleData.Shuffle();


            //merge...
            lblProgress.Text = "Merging data...";
            Application.DoEvents();

            for (int i = 0; i < fileData.Count; i++)
            {
                var scrambleIndex = 0;
                INPUT_FILE_COLUMNS.ForEach(fileColumn => {
                    fileData[i][fileColumn.Index] = scrambleData[i][scrambleIndex++];
                });
            }

            //output...
            lblProgress.Text = "Saving output file...";
            Application.DoEvents();

            var outputFile = Path.Combine(Path.GetDirectoryName(inputFile), $"SCRAMBLED-{Path.GetFileNameWithoutExtension(inputFile)}.txt");

            fileData.Insert(0, header);
            System.IO.File.WriteAllText(outputFile, GetAsText(fileData), Encoding.UTF8);
            System.Diagnostics.Process.Start("notepad.exe", outputFile);
        }

        #region Helpers

        string GetAsText(List<string[]> fileData)
        {
            var sb = new StringBuilder();

            fileData.ForEach(x =>
            {
                sb.AppendLine(string.Concat("DRAFT\t", string.Join("\t", x.ToList().Select(x => x))));
            });

            return sb.ToString();
        }

        void ValidateColumnHeaders(List<FileColumn> fileColumns, string[] rowData)
        {
            var missingColumns = new List<string>();

            foreach (var fileColumn in fileColumns)
            {
                var colName = fileColumn.Name;

                var index = Array.FindIndex(rowData, x => x.Equals(colName, StringComparison.OrdinalIgnoreCase));

                if (index != -1)
                    fileColumn.Index = index;
                else
                {
                    if (fileColumn.Required)
                        missingColumns.Add(colName);
                }
            }

            if (missingColumns.Count > 0)
            {
                Console.WriteLine($"ERROR: Required column(s) missing: {string.Join(", ", missingColumns)}");
                Environment.Exit(1);
            }
        }

        #endregion

    }
}
