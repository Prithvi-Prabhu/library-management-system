import { Book } from './book.model';
import { Member } from './member.model';

export interface Loan {
  Id: number;
  BookId: number;
  MemberId: number;
  LoanDate: string;
  DueDate?: string;
  Returned: boolean;

  Book?: Book;
  Member?: Member;
}