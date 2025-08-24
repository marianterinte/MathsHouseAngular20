export interface MediaConfig {
  houseImage: string; // main house image
  handImage: string; // pointer/hand icon
  startupVideo: string; // intro video
  startupAudio: string; // intro audio
}

export const defaultMediaConfig: MediaConfig = {
  houseImage: 'assets/images/math_house.png',
  handImage: 'assets/images/access_granted.png',
  startupVideo: 'assets/raw/ZStartingVideo.mp4',
  startupAudio: 'assets/raw/evilwitch.wav',
};
