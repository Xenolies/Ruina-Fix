import csv
import os
import locale

def filename_txt():
    print("本地编码格式为: " + locale.getencoding())
    dir = "."
    f1 = open('FileNames.txt', 'w')
    for i in sorted(os.listdir(dir)):
        try:
            a_name = i.encode('gbk').decode('Shift_JIS')
            print('源文件名: ' + i + ' 修改后将是： ' + a_name)
            f1.write('源文件名:  ' + i + '    -   ' + a_name + '\n')
        except:
            print('**文件名<', i, '>中含有非日语编码的字符，故跳过该文件夹。')
            continue
    f1.close()


def filename_csv():
    print("本地编码格式为: " + locale.getencoding())
    dir = "."
    f1 = open('FileNames.csv', 'w')
    headers = ['原文件名','修改后']
    rows = []
    for i in sorted(os.listdir(dir)):
        try:
            a_name = i.encode('gbk').decode('Shift_JIS')
            print('源文件名: ' + i + ' 修改后将是： ' + a_name)
            row = (i , a_name)
            rows.append(row)
            # f1.write('源文件名:  ' + i + '    -   ' + a_name + '\n')
        except:
            print('**文件名<', i, '>中含有非日语编码的字符，故跳过该文件夹。')
            continue
    writer = csv.writer(f1)
    writer.writerow(headers)
    writer.writerows(rows)
    f1.close()

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
