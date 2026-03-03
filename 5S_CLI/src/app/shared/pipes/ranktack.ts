import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'rankTag',
  standalone: true
})
export class RankTagPipe implements PipeTransform {

  private rankMap = [
    { min: 95, color: 'blue',  text: 'Tốt' },
    { min: 90, color: 'orange', text: 'Khá' },
    { min: 80, color: 'orange', text: 'Trung bình' },
    { min: 0,  color: 'red',    text: 'Kém' }
  ];

  transform(point: number) {
    return this.rankMap.find(x => point >= x.min)!;
  }
}
