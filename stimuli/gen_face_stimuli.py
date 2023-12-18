import cv2
import os
from glob import glob
import argparse
import numpy as np

parser = argparse.ArgumentParser(description='Program for generating tilings for different corridors')
parser.add_argument('--input', type=str, default=None, help='Input directory')
parser.add_argument('--output', type=str, default=None, help='Directory for storing generated stimuli')
parser.add_argument('--num_face', type=int, default=8, help='Number of faces for each corridor')

args = parser.parse_args()

# gather raw data 
data = glob(os.path.join(args.input, '*'))
num_stimuli = len(data) // args.num_face

# assuming all face images have consistent size
img_h, img_w, _ = cv2.imread(data[0]).shape

# save directory
if not os.path.exists(args.output):
    os.makedirs(args.output)

# iteratively generating each stimulus, note the faces are unique and will not be selected repeatitively
for i in range(num_stimuli):
    if len(data)-(i*args.num_face) < args.num_face:
        break
    cur_pool = data[i*args.num_face:(i+1)*args.num_face]
    
    # construct the stimulus by concatenating facial images
    stimuli = np.zeros([img_h, img_w*args.num_face, 3]).astype('int')
    for j in range(args.num_face):
        stimuli[:, j*img_w:(j+1)*img_w] = cv2.imread(cur_pool[j])
    
    cv2.imwrite(os.path.join(args.output, 'stimuli_'+str(i+1)+'.png'), stimuli)

    

