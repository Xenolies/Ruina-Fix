import os
#需要改名的文件夹
filePath = 'C:\\Users\\35367\\Desktop\\RPG_RT\\Ruina\\CN_Fix\\Backdrop'
for i,j,k in os.walk(filePath):
    for name in k:
        # 文件初始名字
        print(name)
        #去除指定字符串
        newName=name.replace("・"," ")
        # 文件的绝对路径
        name = i + "\\" + name
        print(name)
        # 更改后的名字
        print(newName)
        # 更改后的绝对路径
        newName = i + "\\"+ newName
        print(newName)
        os.rename(name, newName)
