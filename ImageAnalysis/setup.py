
#from subprocess import call
#subprocess.call('sudo apt install python-pip3', shell=True)
#subprocess.call('sudo apt-get install python3 -y', shell=True)
#pip3 install -r requirements.txt
'''
import shlex, subprocess

command_line = 'sudo apt install python-pip3'
args = shlex.split(command_line)
subprocess.call(args)

command_line = 'sudo apt install python-six'
args = shlex.split(command_line)
subprocess.call(args)

command_line = 'sudo pip3 install numpy'
args = shlex.split(command_line)
subprocess.call(args)

command_line = 'sudo pip3 install tensorflow'
args = shlex.split(command_line)
subprocess.call(args)

command_line = 'git clone https://github.com/tensorflow/models ./repo'
args = shlex.split(command_line)
subprocess.call(args)

#creamos la carpeta /var/rfcx-espol-imageanalysis
command_line = 'mkdir /var/rfcx-espol-imageanalysis/'
args = shlex.split(command_line)
subprocess.call(args)

#movemos el modelo  a la carpeta /var/rfcx-espol-imageanalysis
command_line = 'mv ./repo/models/tutorials/image/imagenet/classify_image.py /var/rfcx-espol-imageanalysis/'
args = shlex.split(command_line)
subprocess.call(args)

#muevo el code.py a la carpeta /var/rfcx-espol-imageanalysis
command_line = 'mv code.py /var/rfcx-espol-imageanalysis/'
args = shlex.split(command_line)
subprocess.call(args)
'''
#poner
#* */4 * * * root python /var/rfcx-espol-imageanalysis/code.py > /var/rfcx-espol-imageanalysis/log.log  2>&1
ruta = "./repo/crontab"
with open(ruta, mode="a", encoding="utf-8") as fichero:
    fichero.write("\n* */4 * * * root python /var/rfcx-espol-imageanalysis/code.py > /var/rfcx-espol-imageanalysis/log.log  2>&1")
