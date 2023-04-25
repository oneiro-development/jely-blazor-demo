
export const getResizedImage = async (element: HTMLInputElement, maxWidth: number, maxHeight: number, dotnetObject: any) => {
    const result = await new Promise<string>(resolve => {
        const file = element.files![0]
        const reader = new FileReader()
        reader.onload = () => {
            const img = new Image()
            img.onload = () => {
                const canvas = document.createElement('canvas')
                const ctx = canvas.getContext('2d')!
                let width = img.width;
                let height = img.height;

                if (width > height && width > maxWidth) {
                    height *= maxWidth / width;
                    width = maxWidth;
                } else if (height > maxHeight) {
                    width *= maxHeight / height;
                    height = maxHeight;
                }

                canvas.width = width;
                canvas.height = height;
                ctx.drawImage(img, 0, 0, width, height);
                resolve(canvas.toDataURL('image/jpeg').replace(/^data:image\/(png|jpg|jpeg);base64,/, ''))
            }
            img.src = reader.result as string;
        }
        reader.readAsDataURL(file)
    })

    const chunkSize = 32000
    const numberOfChunks = Math.ceil(result.length / chunkSize)

    for(let i = 0; i < numberOfChunks; i++) {
        const start = i * chunkSize
        const end = start + chunkSize
        const chunk = result.substring(start, end)
        await dotnetObject.invokeMethodAsync('ReceiveStringChunk', chunk)
    }
}
