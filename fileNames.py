import csv
import os
import locale


def filename_txt():
    print("本地编码格式为: " + locale.getencoding())
    fileNum = 0
    dir = "."
    path = os.path.abspath('.')
    newpath = path.split('\\')[-2:]
    fileName = newpath[0] + '_' + newpath[1] + '.txt'
    f1 = open(fileName, 'w')
    for i in sorted(os.listdir(dir)):
        try:
            a_name = i.encode('gbk','ignore').decode('Shift_JIS')
            print('源文件名: ' + i + ' 修改后将是： ' + a_name)
            f1.write('源文件名:  ' + i + '    -   ' + a_name + '\n')
            fileNum += 1
        except:
            print('**文件名<', i, '>中含有非日语编码的字符，故跳过该文件夹。')
            continue
    f1.write('\n')
    f1.write('总文件数  - ' + str(fileNum))
    f1.close()
    print ("一共统计了 " + str(fileNum) + "个文件")


def filename_csv():
    print("本地编码格式为: " + locale.getencoding())
    fileNum = 0
    dir = "."
    path = os.path.abspath('.')
    newpath = path.split('\\')[-2:]
    fileName = newpath[0] + '_' + newpath[1] + '.csv'
    f1 = open(fileName, 'w')
    headers = ['原文件名','修改后']
    rows = []
    for i in sorted(os.listdir(dir)):
        try:
            a_name = i.encode('gbk','ignore').decode('Shift_JIS')
            print('源文件名: ' + i + ' 修改后将是： ' + a_name)
            row = (i , a_name)
            rows.append(row)
            fileNum += 1
            # f1.write('源文件名:  ' + i + '    -   ' + a_name + '\n')
        except:
            print('**文件名<', i, '>中含有非日语编码的字符，故跳过该文件夹。')
            continue
    writer = csv.writer(f1)
    writer.writerow(headers)
    writer.writerows(rows)
    endRow = [('总文件数',str(fileNum))]
    writer.writerows(endRow)
    f1.close()
    print ("一共统计了 " + str(fileNum) + "个文件")

def main():
    print('当前文件夹为: ' +os.getcwd())
    bool =  input("请选择最终输出的文件格式: "
                  "1) txt 2) SCV: \n")
    if bool == '1':
        filename_txt()
    if bool == '2':
        filename_csv()


if __name__ == '__main__':
    main()
