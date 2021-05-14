/*
 * Created by SharpDevelop.
 * User: guilh
 * Date: 13/08/2017
 * Time: 01:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SortFiles
{
	class Program
	{
		
		[STAThread]
		public static void Main(string[] args)
		{
			FolderBrowserDialog fbd;
			String pastaDestino = null;
			List<String> pastas = new List<string>();
			Boolean finish = false;
			Boolean removerArquivosOriginais = false;
			Boolean removerPastasVazias = false;
			while (!finish){
				Console.Clear();
				Console.WriteLine("Selecione a opção desejada:\r\n");
				Console.WriteLine("0- Sair");
				Console.WriteLine("--------------------------------------------------\r\nPasta destino:");
				Console.WriteLine("1- Selecionar\r\n\t["+ (pastaDestino ?? "Não Selecionada")+"]");
				Console.WriteLine("--------------------------------------------------\r\nPastas para organizar:");
				if (pastas.Count==0){
					Console.WriteLine("\t[Nenhuma pasta selecionada]");
				}else{
					for (int i=0; i<pastas.Count; i++){
						Console.WriteLine("\t["+pastas[i]+"]");
					}
				}
				Console.WriteLine("2- Adicionar");
				Console.WriteLine("3- Remover");
				Console.WriteLine("--------------------------------------------------");
				Console.WriteLine("4- Remover arquivos originais? "+(removerArquivosOriginais?"Sim":"Não"));
				Console.WriteLine("5- Remover pastas vazias? "+(removerPastasVazias?"Sim":"Não"));
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
						case 2:
							fbd = new FolderBrowserDialog();
							fbd.Description = "Selecione uma pasta para organizar.";
							if (fbd.ShowDialog() == DialogResult.OK) 
						    {
								String selected = fbd.SelectedPath;
								if (pastas.IndexOf(selected)==-1){
									pastas.Add(selected);
								}
							}
							break;
						case 3:
							Console.Clear();
							if (pastas.Count>0){
								Console.WriteLine("Selecione a pasta a remover:");
								Console.WriteLine("0 - Voltar\r\n");
								for (int i=0; i<pastas.Count; i++){
									Console.WriteLine((i+1)+" - "+pastas[i]);
								}
								
								Console.Write("Opção: ");
								String opcaoString2 = Console.ReadLine();
								int opcao2 = int.Parse(opcaoString2);
								if (opcao2!=0){
									try{
										pastas.RemoveAt(opcao2-1);
									} catch{
									}
								}
							}
							break;
						case 4:
							removerArquivosOriginais = !removerArquivosOriginais;
							break;
						case 5:
							removerPastasVazias = !removerPastasVazias;
							break;
						case 9:
							if (pastaDestino != null && pastas.Count>0){
								List<String> arquivos = new List<string>();
								foreach (String pasta in pastas){
									Console.WriteLine("Pesquisando arquivos em "+pasta);
									arquivos.AddRange(getArquivosRecursivamente(pasta));
								}
								
								foreach (String arquivo in arquivos){
									String ext = Path.GetExtension(arquivo).Replace(".","");
									String novoNome;
									if (string.IsNullOrEmpty(ext)) {
										if (!Directory.Exists(pastaDestino + "\\none")) {
											Directory.CreateDirectory(pastaDestino + "\\none");
										}
										String nomeArquivo = Path.GetFileNameWithoutExtension(arquivo);
										int j = -1;
										novoNome = pastaDestino + "\\none\\" + nomeArquivo;
										while (true) {
											j++;
											if (File.Exists(novoNome)) {
												novoNome = pastaDestino + "\\none\\" + nomeArquivo + "_" + j;
											} else {
												break;
											}
										}
									} else {
										if (!Directory.Exists(pastaDestino+"\\"+ext)){
											Directory.CreateDirectory(pastaDestino+"\\"+ext);
										}
										String nomeArquivo = Path.GetFileNameWithoutExtension(arquivo);
										int j=-1;
										novoNome = pastaDestino+"\\"+ext+"\\"+nomeArquivo+"."+ext;
										while (true){
											j++;
											if (File.Exists(novoNome)){
												novoNome = pastaDestino+"\\"+ext+"\\"+nomeArquivo+"_"+j+"."+ext;
											} else {
												break;
											}
										}
									}
									Console.WriteLine((removerArquivosOriginais ? "Movendo" : "Copiando") + " arquivo '" + arquivo + "' para " + novoNome);
									try {
										if (removerArquivosOriginais)
											File.Move(arquivo, novoNome);
										else
											File.Copy(arquivo, novoNome);
									} catch {
										
									}
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
		
		static List<String> getDiretoriosRecursivamente(string sDir)
		{
			List<String> diretorios = new List<string>();
		    try
		    {
		        foreach (string d in Directory.GetDirectories(sDir))
		        {
		            diretorios.AddRange(getDiretoriosRecursivamente(d));
		        }
		    }
		    catch (System.Exception excpt)
		    {
		        Console.WriteLine(excpt.Message);
		    }
		    return diretorios;
		}
		
		static List<String> getArquivosRecursivamente(string sDir)
		{
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