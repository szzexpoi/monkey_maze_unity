import cv2
import json
import numpy as np
from glob import glob
import os
import argparse

parser = argparse.ArgumentParser(description='Converting images to start/end points for maze wall')
parser.add_argument('--project_dir', type=str, default=None, help='Directory to the project')
parser.add_argument('--input', type=str, default=None, help='Input image for the 2D maze')
args = parser.parse_args()

def preprocess_img(img):
    """ Function for preprocessing raw image data
        Inputs:
            img: 2D numpy array of the raw image
        Outputs:
            img: 2D numpy array of processed image
    """

    # binarize the image (reverse black and white)
    # img = cv2.resize(img, (200, 200)) # use fixed size for now
    img[img<=10] = 1
    img[img!=1] = 0

    return img


def consolidate_points(points, vertical=False):
    """ Function for converting dictionary data into list.
        Inputs:
            points: Dictionary storing the start/end points,
                    where the key are (start, end) location for the main axis
                    (vertical or horital), value is a list storing the secondary
                    axis.
            vertical: Specifying if the current data is for vertical walls
        Outputs:
            filtered_points: A list storing the start/end point
                            locations for different walls. The format is
                            (start_x, start_y, end_x, end_y)
    """

    filtered_points = []
    for i, duplicated_set in enumerate(points):
        # processing horizontal walls
        if not vertical:
            start_x, end_x = duplicated_set
            if np.abs(start_x-end_x)<=10:
                continue
            for y in points[duplicated_set]:
                # filtered_points.append([int(start_x), int(y), int(end_x), int(y)])
                point_dict = {
                            "start_x": int(start_x),
                            "start_y": int(y),
                            "end_x": int(end_x),
                            "end_y": int(y)
                            }
                filtered_points.append(point_dict)
        else:
            start_y, end_y = duplicated_set
            if np.abs(start_y-end_y)<=10:
                continue
            for x in points[duplicated_set]:
                # filtered_points.append([int(x), int(start_y), int(x), int(end_y)])
                point_dict = {
                            "start_x": int(x),
                            "start_y": int(start_y),
                            "end_x": int(x),
                            "end_y": int(end_y)
                            }
                filtered_points.append(point_dict)
    return filtered_points


# preprocessing image
img = cv2.imread(args.input)[:, :, 0].astype('float32')
img = preprocess_img(img)
cv2.imwrite('check.png', img*255)

walls = []
direction = []

# horizontal wall detection
horizontal_wall = dict()
for i in range(len(img)):
    prev_x = -1
    for j in range(len(img[0])):
        # start point
        if prev_x == -1 and img[i, j]:
            prev_x = int(j/10) * 10
        # end point
        elif prev_x != -1 and not img[i,j]:
            if (prev_x, int((j-1)/10)*10) not in horizontal_wall:
                horizontal_wall[(prev_x, int((j-1)/10)*10)] = [int(i/10)*10]
            else:
                horizontal_wall[(prev_x, int((j-1)/10)*10)].append(int(i/10)*10)
            prev_x = -1
        else:
            continue
# remove duplicated walls caused by lines with non-uniform length
horizontal_wall_filtered = dict()
for k in horizontal_wall:
    horizontal_wall[k] = sorted(list(set(horizontal_wall[k])))
    if len(horizontal_wall[k])>1: 
        prev_idx = 0 # use an index to record the start of a continuous sequence
        horizontal_wall_filtered[k] = []
        for i in range(1, len(horizontal_wall[k])):
            if horizontal_wall[k][i] != horizontal_wall[k][i-1]+1:
                horizontal_wall_filtered[k].append(np.min(horizontal_wall[k][prev_idx:i]))
                prev_idx = i
            if (i == len(horizontal_wall[k]) - 1):
                if len(horizontal_wall_filtered[k]) == 0:
                    horizontal_wall_filtered[k].append(np.min(horizontal_wall[k][prev_idx:]))
                else:
                    if (horizontal_wall[k][-1]-horizontal_wall_filtered[k][-1])>10:
                        horizontal_wall_filtered[k].append(horizontal_wall[k][-1])
    else:
        horizontal_wall_filtered[k] = horizontal_wall[k]

walls.extend(consolidate_points(horizontal_wall_filtered, False))
direction.extend([1]*len(walls)) # 1 indicate horizontal wall, 0 otheriwse

# randomly select a vertical corridor for the starting position (spawn between two walls)
valid_y = False
while not valid_y:
    start_idx = int(np.random.choice(np.arange(len(walls)-1), 1)[0])
    corridor_width = walls[start_idx+1]['start_y'] - walls[start_idx]['start_y'] # compute the corridor width
    if corridor_width > 20:
        init_y = int(walls[start_idx]['start_y'] + corridor_width/2)
        valid_y = True

# vertical wall detection
vertical_wall = dict()
for j in range(len(img[0])):
    prev_y = -1
    for i in range(len(img)):
        # start point
        if prev_y == -1 and img[i, j]:
            prev_y = int(i/10)*10
        # end point
        elif prev_y != -1 and not img[i,j]:
            if (prev_y, int((i-1)/10)*10) not in vertical_wall:
                vertical_wall[(prev_y, int((i-1)/10)*10)] = [int(j/10)*10]
            else:
                vertical_wall[(prev_y, int((i-1)/10)*10)].append(int(j/10)*10)
            prev_y = -1
        else:
            continue

# remove duplicated walls caused by lines with non-uniform length
vertical_wall_filtered = dict()
for k in vertical_wall:
    vertical_wall[k] = sorted(list(set(vertical_wall[k])))
    if len(vertical_wall[k])>1:
        prev_idx = 0 # use an index to record the start of a continuous sequence
        vertical_wall_filtered[k] = []
        for i in range(1, len(vertical_wall[k])):
            if vertical_wall[k][i] != vertical_wall[k][i-1] + 1:
                vertical_wall_filtered[k].append(np.min(vertical_wall[k][prev_idx:i]))
                prev_idx = i
            if i == len(vertical_wall[k]) - 1:
                vertical_wall_filtered[k].append(vertical_wall[k][-1])
                if len(vertical_wall_filtered[k]) == 0:
                    vertical_wall_filtered[k].append(np.min(vertical_wall[k][prev_idx:]))
                else:
                    if (vertical_wall[k][-1]-vertical_wall_filtered[k][-1])>10:
                        vertical_wall_filtered[k].append(vertical_wall[k][-1])
    else:
        vertical_wall_filtered[k] = vertical_wall[k]
walls.extend(consolidate_points(vertical_wall_filtered, True))
direction.extend([0]*(len(walls)-len(direction)))

# visualize the processed data for validation
height, width = img.shape
vis_img = np.zeros([height, width])*255
for wall in walls:
    start_x, start_y, end_x, end_y = wall['start_x'], wall['start_y'], wall['end_x'], wall['end_y'], 
    cv2.line(vis_img, (start_x, start_y), (end_x, end_y), (255, 255, 255))

# randomly select a horizontal positions for the startig position
pool = []
for x in np.arange(width):
    if vis_img[init_y, x-corridor_width:x+corridor_width].sum() == 0:
        pool.append(x) 
init_x = np.random.choice(pool, 1)[0]
cv2.imwrite('test.png', vis_img)

# store the available stimuli in the json file
all_stimuli = glob(os.path.join(args.project_dir, 'Assets', 'Resources', 'stimuli', '*.png'))

# randomized order
random_idx = np.random.choice(np.arange(len(all_stimuli)), len(all_stimuli), replace=False)
record_stimuli = [{"stimuli_path": os.path.join('stimuli', os.path.basename(all_stimuli[i])[:-4])} for i in random_idx]

save_walls = {"walls": walls, "directions": direction, "start_position": 
              {"start_x": int(init_x), "start_y": int(init_y)}, "stimuli_dir": record_stimuli}

with open(os.path.join(args.project_dir, 'maze_layout.json'), 'w') as f:
    json.dump(save_walls, f)