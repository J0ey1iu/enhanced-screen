import cv2
import numpy as np
import dlib
import socket
import time

from typing import List
from collections import deque

# from scipy.interpolate import interp1d

class Buffer:
    """
    Temporary points container
    """

    def __init__(self, size: int):
        self.pts = deque([])
        self.size = size
        
        self.UDP_IP = "127.0.0.1"
        self.UDP_PORT = 5065
        print("UDP target IP: {}".format(self.UDP_IP))
        print("UDP target port: {}".format(self.UDP_PORT))
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    def add_point(self, point: List[float]):
        x, y = point
        if len(self.pts) < self.size:
            self.pts.append(point)
        else:
            self.send_point(self.pts.popleft())
            self.pts.append(point)
    
    def send_point(self, point: List[float]):
        bin_data = bytes("{},{}".format(x, y), encoding='utf-8')
        self.sock.sendto(bin_data, (self.UDP_IP, self.UDP_PORT))

    def send_null(self):
        bin_data = bytes("{},{}".format('no', 'no'), encoding='utf-8')
        self.sock.sendto(bin_data, (self.UDP_IP, self.UDP_PORT))


if __name__ == "__main__":
    cap = cv2.VideoCapture(0)

    detector = dlib.get_frontal_face_detector()
    predictor = dlib.shape_predictor("shape_predictor_68_face_landmarks.dat")

    b = Buffer(5)
    num_frame = 0

    start = time.time()
    while True:
        num_frame += 1
        _, frame = cap.read()
        resized = cv2.resize(frame, (16 * 30, 9 * 30))
        gray = cv2.cvtColor(resized, cv2.COLOR_BGR2GRAY)

        faces = detector(gray)      
        if len(faces) == 0:
            b.send_null()
            print("no message sent.")
        end = time.time()
        for face in faces:
            landmarks = predictor(gray, face)
            for n in range(0, 68):
                x = landmarks.part(n).x
                y = landmarks.part(n).y
                if n == 33:
                    cv2.circle(resized, (x, y), 2, (0, 0, 255), -1)
                    b.add_point([x, y])
                else:
                    cv2.circle(resized, (x, y), 2, (0, 255, 0), -1)
        
        
        print("fps: {0: .2f}".format(num_frame / (end - start)))
        
        # TODO (Jiayu): Realtime trajectory optimization

        cv2.imshow("Frame", resized)

        key = cv2.waitKey(1)
        if key == 27:
            break