type Webcam = {
    invokeMethodAsync(method: string, chunk: string): void
    invokeMethodAsync(method: string): void
}

let stream: MediaStream | null;

export const captureWebcam = async (videoElement: HTMLVideoElement, canvasElement: HTMLCanvasElement, dotnetObject: Webcam) => {
    stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false })
    videoElement.srcObject = stream
    await videoElement.play()
    await dotnetObject.invokeMethodAsync('Loaded')
    videoElement.onloadedmetadata = () => {  }
}

export const stopWebcam = (videoElement: HTMLVideoElement) => {
    videoElement.srcObject = null
    videoElement.pause()
    videoElement.load()
    stream?.getTracks().forEach(track => track.stop() )
    stream = null
}

export const captureImage = async (videoElement: HTMLVideoElement, canvasElement: HTMLCanvasElement, dotnetObject: Webcam) => {
    canvasElement.width = videoElement.videoWidth
    canvasElement.height = videoElement.videoHeight
    const ctx = canvasElement.getContext('2d')
    ctx!.drawImage(videoElement, 0, 0, canvasElement.width, canvasElement.height)
    const dataUrl = canvasElement.toDataURL('image/jpeg')
    const base64 = dataUrl.replace(/^data:image\/(png|jpg|jpeg);base64,/, '')
    const chunkSize = 32000
    const numberOfChunks = Math.ceil(base64.length / chunkSize)

    for(let i = 0; i < numberOfChunks; i++) {
        const start = i * chunkSize
        const end = start + chunkSize
        const chunk = base64.substring(start, end)
        await dotnetObject.invokeMethodAsync('ReceiveStringChunk', chunk)
    }
}