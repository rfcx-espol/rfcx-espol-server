from statistics import mean
import re

def get_tensor(line):
    match = re.match("([^\(\)=)]+).([^\(\)=)]+).([^\(\)=)]+)", line)
    groups = match.groups()
    return groups[0].strip(), float(groups[2])

proc_path = "processed.csv"
output = "output.txt"
data = {}
with open(output, 'r') as raw:
    lines = raw.readlines()
    last_species = ""
    i = 0
    while i < len(lines):
        line = lines[i].strip()
        if not line.startswith("ESPECIE"):
            specie, score = get_tensor(line)
            """if specie in data[last_species]:
                data[last_species][specie].append(score)
            else:
                data[last_species][specie] = [score]"""
            data[last_species][specie] = data[last_species].get(specie, []).append(score)
            i+=5 #INCOMPLETE
        else:
            last_species = line.split(": ")[1]
            data[last_species] = {}
            i+=1

    with open(proc_path, 'w') as processed:
        processed.write("ESPECIE;T1;T2;T3;T4;T5;IDEN;PROB\n")
        for real_species in data:
            identified = False
            for possible_species in data[real_species]:
                avg = mean(data[real_species][possible_species])
                if avg > 0.5:
                    processed.write("%s;%s;%s\n" %(real_species, possible_species, avg))
                    identified = True
            if not identified:
                processed.write("%s;NO IDENTIFICADA;-1\n" %real_species)
