export interface Book {
  Id: number;
  Title: string;
  Author: string;
  Genre?: string;
  YearPublished: number;
  AvailableCopies: number;
}