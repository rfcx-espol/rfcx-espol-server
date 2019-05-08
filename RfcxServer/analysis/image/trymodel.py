from os.path import isfile
from os.path import basename
from os.path import dirname
from os import listdir
import subprocess
import re

tensorpath = '../tensorflowRepo/tutorials/image/imagenet/classify_image.py'
animalespath = '../../../../Downloads/ANIMALES EN GENERAL/'
output = 'output.csv'


def get_content(path_):
        """Retorna el contenido de una carpeta como una lista de strings"""
        return [path_+x if isfile(path_+x) else path_+x+"/"
                for x in listdir(path_)]


def tensoroutput(image):
        """Retorna el output del script de tensorflow como un string"""
        # result = subprocess.run(['python3', tensorpath, '--image_file',
        # image], stdout=subprocess.PIPE)
        result = subprocess.run('python3 %s --image_file %s'
                                % (tensorpath, image),
                                shell=True, stdout=subprocess.PIPE)
        return result.stdout


def extractdata(line):
    match = re.match('([^\(\)=)]+).([^\(\)=)]+).([^\(\)=)]+)', line)
    groups = match.groups()
    return groups[0].strip(), float(groups[2])


def structureoutput(output):
        """
        Procesa el output y lo transforma a un diccionario donde:
        la clave es el animal y el valor es la probabilidad
        """
        D = {}
        lines = output.decode().split('\n')[:-1]
        for line in lines:
                data = extractdata(line)
                D[data[0]] = data[1]
        return D

def structure_lists_output(output):
        """
        Procesa el output y lo transforma a dos listas donde:
        la primera tiene las especies y la segunda la probabilidad
        correspondiente
        """
        species = []
        probs = []
        lines = output.decode().split('\n')[:-1]
        for line in lines:
                data = extractdata(line)
                species.append(data[0])
                probs.append(data[1])
        return species, probs


if __name__ == "__main__":
	with open(output, 'w') as f:
		animales = get_content(animalespath)
		f.write('PATH;T1;T2;T3;T4;T5;IDEN;PROB\n')
		for folder in animales:
		        files = get_content(folder)
		        for file in files:
		                filepath = '"' + file + '"'
		                writepath = basename(dirname(file))+'/'+basename(file)
		                raw_output = tensoroutput(filepath)
		                pro_output = structureoutput(raw_output)
		                f.write('%s' % writepath)
		                for key in pro_output:
		                        f.write(';%s' % key)
		                f.write('\n')
		                f.flush()
