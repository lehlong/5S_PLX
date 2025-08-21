import { BaseFilter } from '../base.model'

export class NewsFilter extends BaseFilter {
  id: string = ''
  title: string = ''
  isActive?: boolean | string | null
  SortColumn: string = ''
  IsDescending: boolean = true
}
