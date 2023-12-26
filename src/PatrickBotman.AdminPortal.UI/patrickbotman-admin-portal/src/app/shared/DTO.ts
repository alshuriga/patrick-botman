export interface Page<T> {
    items: T[],
    collectionSize: number
};

export interface GifDTO {
    id: number, 
    url: string
}

export interface ChatDTO {
    id: number
}