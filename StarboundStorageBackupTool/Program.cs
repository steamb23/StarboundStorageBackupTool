using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StarboundStorageBackupTool
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "스타바운드 원터치 세이브 백업 도구";
            string filePath;

            try
            {
                filePath = File.ReadAllText("starboundDir.txt");
            }
            catch
            {
                Console.WriteLine("starboundDir.txt를 읽을 수 없습니다.");
                Console.WriteLine("스타바운드 설치 경로를 수동으로 입력하세요.");
                filePath = Console.ReadLine();
                File.WriteAllText("starboundDir.txt", filePath);
            }
            Console.Write("경로: ");
            Console.WriteLine(filePath);

            Console.WriteLine("해당 경로가 스타바운드경로가 맞는지 확인합니다...");
            if (File.Exists(filePath + @"\assets\packed.pak"))
            {
                Console.WriteLine("확인되었습니다.");
            }
            else
            {
                Console.WriteLine("오류: 스타바운드 경로가 아닙니다.");
                Console.ReadKey(true);
                return;
            }

            string storagePath = filePath + @"\storage";
            Console.Write("세이브 경로: ");
            Console.WriteLine(storagePath);
            Console.WriteLine("세이브 파일 복사를 시작합니다. 잠시만 기다려주세요.");
            Copy(storagePath, storagePath + " backup - " + DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH'_'mm'_'ss"));
        }
        static void Copy(string sourcePath, string destPath)
        {
            Directory.EnumerateDirectories(sourcePath);
            DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
            DirectoryInfo destDir = new DirectoryInfo(destPath);

            // 상대 폴더 생성
            destDir.Create();
            // 파일 복사
            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    Copy(sourceDir, destDir);
                }
                catch (IOException)
                {
                    Console.WriteLine("현재 {0}회 재시도합니다. 5회 재시도 후에도 안되면 종료됩니다.", i);
                    continue;
                }
                break;
            }
        }
        static void Copy(DirectoryInfo sourceDir, DirectoryInfo destDir)
        {
            foreach (var sourceFile in sourceDir.GetFiles())
            {
                Console.Write("파일 복사: ");
                Console.WriteLine(sourceFile.FullName);
                try
                {
                    sourceFile.CopyTo(destDir.FullName + "\\" + sourceFile.Name);
                }
                catch (IOException e)
                {
                    if (sourceFile.Name == "universe.lock")
                    {
                        Console.WriteLine("universe.lock 제외됨.");
                    }
                    else
                    {
                        Console.WriteLine("복사가 실패했습니다. 5초후 재개합니다.");
                        Thread.Sleep(5000);
                        throw e;
                    }
                }
            }
            foreach (var sourceSubDir in sourceDir.GetDirectories())
            {
                Console.Write("디렉토리 복사: ");
                Console.WriteLine(sourceSubDir.FullName);
                var destSubDir = destDir.CreateSubdirectory(sourceSubDir.Name);
                Copy(sourceSubDir, destSubDir);
            }
        }
    }
}
