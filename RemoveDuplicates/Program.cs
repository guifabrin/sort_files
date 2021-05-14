/*
 * Created by SharpDevelop.
 * User: guilh
 * Date: 13/08/2017
 * Time: 15:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace RemoveDuplicates
{
	class Program
	{		
		
		[STAThread]
		public static void Main(string[] args)
		{
			FolderBrowserDialog fbd;
			String pastaDestino = null;
			Boolean finish = false;
			while (!finish){
				Console.Clear();
				Console.WriteLine("Selecione a opção desejada:\r\n");
				Console.WriteLine("0- Sair");
				Console.WriteLine("--------------------------------------------------\r\nPasta destino:");
				Console.WriteLine("1- Selecionar\r\n\t["+ (pastaDestino ?? "Não Selecionada")+"]");
				Console.WriteLine("--------------------------------------------------\r\n");
				Console.WriteLine("9- Executar");
				Console.Write("Opção: ");
				String opcaoString = Console.ReadLine();
				try{
					int opcao = int.Parse(opcaoString);
					switch (opcao){
						case 1:
							fbd = new FolderBrowserDialog();
							fbd.Description = "Selecione a pasta de destino.";
							if (fbd.ShowDialog() == DialogResult.OK) 
						    {
								pastaDestino = fbd.SelectedPath;
							}
							break;
						case 9:
							if (pastaDestino != null){
								List<String> arquivos = getArquivosRecursivamente(pastaDestino+"/");
								Dictionary<String,List<String>> arquivosDuplicados = new Dictionary<string, List<string>>();
								foreach (String arquivo in arquivos){
									Console.WriteLine("Analisando arquivo ["+arquivo+"]");
									using (var crypt = MD5.Create())
									{
									    using (var stream = File.OpenRead(arquivo))
									    {
									    	byte[] arr = crypt.ComputeHash(stream);
									    	String md5 = BitConverter.ToString(arr).Replace("-","‌​").ToLower();
									    	if (arquivosDuplicados.ContainsKey(md5)){
									    		arquivosDuplicados[md5].Add(arquivo);
									    	} else {
									    		arquivosDuplicados.Add(md5, new List<string>());
									    		arquivosDuplicados[md5].Add(arquivo);
									    	}
									    }
									}
								}
								
								foreach (List<String> value in arquivosDuplicados.Values){
									if (value.Count > 1){
										for (int i=1; i<value.Count;i++){
											Console.WriteLine("Removendo arquivo ["+value[i]+"]");
											File.Delete(value[i]);
										}
									}
								}
								
								arquivos = getArquivosRecursivamente(pastaDestino+"/");
								
								foreach (String arquivo in arquivosDuplicados.Values){
									DateTime dt = File.GetCreationTime(arquivo);
									String path = Path.GetDirectoryName(arquivo);
									String extension = Path.GetExtension(arquivo);
									File.Move(arquivo, path+"/"+dt.Year+dt.Month+dt.Date+"_"+dt.Hour+dt.Minute+dt.Second+extension);
								}
								
							}
							break;
						case 0:
							finish = true;
							break;
					}
				} catch {
					Console.Write("Opção não disponível");
				}
			}
		}
		
		static List<String> getArquivosRecursivamente(string sDir)
		{
			Console.Write(sDir);
			List<String> arquivos = new List<string>();
		    try
		    {
		        foreach (string d in Directory.GetDirectories(sDir))
		        {
		            foreach (string f in Directory.GetFiles(d))
		            {
		            	arquivos.Add(f);
		            }
		            arquivos.AddRange(getArquivosRecursivamente(d));
		        }
		        
		        foreach (string f2 in Directory.GetFiles(sDir))
	            {
	            	arquivos.Add(f2);
	            }
		    }
		    catch (System.Exception excpt)
		    {
		        Console.WriteLine(excpt.Message);
		    }
		    return arquivos;
		}
	}
}