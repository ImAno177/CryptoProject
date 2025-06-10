using Crypto.APIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypto.HelperClass
{
    internal class LoadFiles
    {
        static public async void LoadAllFiles(DataGridView dataGridView)
        {
            try
            {
                var fileService = new FileService();
                var files = await fileService.GetFilesAsync();
                dataGridView.Rows.Clear();

                foreach (var file in files)
                {
                    dataGridView.Rows.Add(
                        file.file_id,
                        file.original_filename,
                        file.size,
                        file.uploaded_at
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading files: " + ex.Message);
            }
        }
    }
}
