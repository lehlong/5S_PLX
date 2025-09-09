import { BaseFilter } from '../base.model'

export class KiKhaoSatFilter extends BaseFilter {
  id: string = ''
  name: string = ''
  isActive?: boolean | string | null
  SortColumn: string = 'startDate'
  IsDescending: boolean = true
  headerId: string = ''
}
