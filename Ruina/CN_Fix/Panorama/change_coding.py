import os

def ask(b_path,a_path):
    if input('-原始文件名：{} \n-修改后将是：{}\
\n**↑请检查（跳过请输入s，确认则直接敲回车）：'.format(b_path,a_path)) == 's':   
        return False
    else:
        return True

def filename_coding():
    dir=input('请输入文件夹路径：')
    for i in os.listdir(dir):
        try:
            a_name=i.encode('gbk').decode('Shift_JIS')
        except:
            print('**文件名<',i,'>中含有非日语编码的字符，故跳过该文件夹。')
            continue
        b_path=dir+'\\'+i
        a_path=dir+'\\'+a_name
        if ask(b_path,a_path):
            os.rename(b_path,a_path)
            print('√')

    input('完成！输入任意键继续：')

def content_coding():
    dir=input('输入要修改文本编码的文件所在目录（将自动读取txt文件）：')
    files=[dir+'\\'+i for i in os.listdir(dir) if i[-4:]=='.txt']
    for txt_file in files:
        print('-'*100)
        r_file=open(txt_file,encoding='Shift_JIS')
        try:
            a_content=r_file.read()
        except UnicodeDecodeError:
            print('**文件【{}】不是日语编码，故跳过。'.format(txt_file))
            continue
        if  input('-文件【{}】\n-修改编码后的内容↓：\n{}\
        \n**请确认生成新文件（取消则输入n，继续则直接敲回车）：'.format(txt_file,a_content)) == 's':
            print('**已取消！')
        else:
            new_file=txt_file[:-4]+' '+'.txt'
            w_file=open(new_file,'w',encoding='utf8')
            w_file.write(a_content)
            w_file.close()
            print('{}√ （新生成的文件名是原文件名最后多一个空格）'.format(new_file))

    input('\n\n\n完成！输入任意键继续：')
    
def main():
    while True:
        keyword=input("修改文件名乱码（filename）还是修改文件内容(content)乱码\n【请输入f(filename)/c(content)】【输入q可退出】：")
        if keyword[0]=='f':
            filename_coding()
        elif keyword[0]=='c':
            content_coding()
        elif keyword[0]=='q':
            break
        else:
            print('错误输入，请重新输入！')
            
        

if __name__ =='__main__':
    main()
