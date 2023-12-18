# Monkey Maze Project
This is an ongoing project for generating 3D maze environment for monkey experiments. The main idea is to take in a top-down view of a customized maze, stored as a standard image, and then automatically generate the maze in Unity. The stimuli within the maze are created by random selecting a collection of unique faces, and consolidating them as tiles. 

### Stimuli Preparation
To generate the stimuli, simply call the following script:
```
cd stimuli
python gen_face_stimuli.py --input SRC_DIR --output TAR_DIR --num_face N
```
where SRC_DIR and TAR_DIR specify the directory to the source facial images and directory for storing the results, respectively. N is the hyperparameter controlling the number of faces appeared within a single stimuli. The source images we used can be found [here](https://drive.google.com/file/d/1aTIBNNa3ZUD3rUQup8cb4SrbaXiM9Al_/view?usp=sharing).

Upon generating the stimuli, copy them to the Unity Asset folder: 
```
Assets/Resources/stimuli
```

### Maze Layout
To create the layout of the maze, you can draw it with any tool you like and save it as an image file (e.g., png). For demonstration, I created mine using a online tool called [draw.io](https://app.diagrams.net/). With the image, you can generate the Unity-compatible file with:
```
cd stimuli
python img2points.py --project_dir ../../MonekyMaze3D --input IMG --output ./
```
where IMG is the image file for maze layout.

### Unity
The Unity code is tested with Unity 2022.3.8f1 on MacOS, but works for all platforms. When running the environment, the movement can be controlled with arrow keys. Later on, the movement control will be migrated to Joystick.