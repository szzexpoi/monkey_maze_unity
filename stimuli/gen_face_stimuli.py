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
# each stimuli set contain three sub-stimuli (for walls of different lengths) 
num_stimuli = len(data) // (args.num_face+args.num_face//2+1) 

# assuming all face images have consistent size
img_h, img_w, _ = cv2.imread(data[0]).shape

# iteratively generating each stimulus, note the faces are unique and will not be selected repeatitively
offset_h = 150
offset_w = 100
border = 150
type2num = {'long': args.num_face, 'medium': args.num_face//2, 'short': 1}
counter = 0
for i in range(num_stimuli):
    for stim_type in ['long', 'medium', 'short']:
        # save directory
        if not os.path.exists(os.path.join(args.output, stim_type)):
            os.makedirs(os.path.join(args.output, stim_type))

        cur_num_face = type2num[stim_type]
        if len(data)-counter < cur_num_face:
            break
        cur_pool = data[counter:counter+cur_num_face]
        
        # construct the stimulus by concatenating facial images
        stimuli = np.ones([img_h+offset_h*2, border*2+img_w*cur_num_face+offset_w*(cur_num_face-1), 3]).astype('int')*128
        for j in range(cur_num_face):
            if j == 0:
                stimuli[offset_h:img_h+offset_h, border+j*img_w:border+(j+1)*img_w] = cv2.imread(cur_pool[j])
            else:
                stimuli[offset_h:img_h+offset_h, border+j*(img_w+offset_w):border+j*(img_w+offset_w)+img_w] = cv2.imread(cur_pool[j])

        cv2.imwrite(os.path.join(args.output, stim_type, 'stimuli_'+str(i+1)+'.png'), stimuli)
        counter += cur_num_face
    

